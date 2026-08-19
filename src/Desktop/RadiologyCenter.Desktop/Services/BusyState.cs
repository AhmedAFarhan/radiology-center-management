namespace RadiologyCenter.Desktop.Services;

/// <summary>
/// Tracks the number of in-flight backend operations started through
/// <see cref="SafeExecute"/>. The layout renders a global loading overlay
/// while the count is above zero.
/// </summary>
public sealed class BusyState
{
    public static BusyState Instance { get; } = new();

    private int _count;

    public bool IsBusy => _count > 0;

    public event Action? Changed;

    public void Begin()
    {
        _count++;
        Changed?.Invoke();
    }

    public void End()
    {
        if (_count > 0)
            _count--;
        Changed?.Invoke();
    }
}