using Microsoft.Data.SqlClient;

namespace RadiologyCenter.Clinic.Security;

/// <summary>An interrupted SQL session must sign in again before business operations resume.</summary>
public sealed class ClinicSessionGuard
{
    internal SqlConnection? Lease { get; set; }
    internal string? CredentialVersion { get; set; }
    private readonly SemaphoreSlim _gate = new(1, 1);
    private readonly object _operationGate = new();
    private readonly TaskCompletionSource _drained = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private int _activeOperations;
    private bool _closing;

    internal IDisposable? EnterOperation()
    {
        lock (_operationGate)
        {
            if (_closing) return null;
            _activeOperations++;
            return new OperationLease(this);
        }
    }

    internal Task StopAndDrainAsync()
    {
        lock (_operationGate)
        {
            _closing = true;
            if (_activeOperations == 0) _drained.TrySetResult();
            return _drained.Task;
        }
    }

    private void ExitOperation()
    {
        lock (_operationGate)
            if (--_activeOperations == 0 && _closing) _drained.TrySetResult();
    }

    private sealed class OperationLease(ClinicSessionGuard owner) : IDisposable
    {
        private ClinicSessionGuard? _owner = owner;
        public void Dispose() => Interlocked.Exchange(ref _owner, null)?.ExitOperation();
    }
    public async Task<bool> IsActiveAsync(CancellationToken ct)
    {
        await _gate.WaitAsync(ct);
        try
        {
            if (Lease?.State != System.Data.ConnectionState.Open) return false;
            await using var command = Lease.CreateCommand(); command.CommandTimeout = 5;
            command.CommandText = "SELECT APPLOCK_MODE('public',N'EGcare.SchemaMaintenance','Session')";
            return (await command.ExecuteScalarAsync(ct)) as string == "Shared";
        }
        catch (SqlException) { return false; }
        finally { _gate.Release(); }
    }
}
