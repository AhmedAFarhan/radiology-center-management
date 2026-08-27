using System.Text.Json;
using MudBlazor;

namespace RadiologyCenter.Desktop.Services;

public static class SafeExecute
{
    public static Task<bool> RunAsync(Func<Task> action, ISnackbar snackbar, Func<string> unreachable)
        => RunAsyncCore(action, snackbar, unreachable, null, useGlobalBusy: true);

    public static Task<bool> RunAsync(Func<Task> action, ISnackbar snackbar, Func<string> unreachable, Action<bool> setBusy)
        => RunAsyncCore(action, snackbar, unreachable, setBusy, useGlobalBusy: false);

    /// <summary>
    /// Runs a value-returning operation without the global busy overlay,
    /// showing <paramref name="unreachable"/> on unexpected failures and the
    /// server-provided message on API errors. Returns default on failure.
    /// </summary>
    public static async Task<T?> RunAsync<T>(Func<Task<T>> action, ISnackbar snackbar, Func<string> unreachable)
    {
        try
        {
            return await action();
        }
        catch (ApiException ex)
        {
            snackbar.Add(FormatError(ex), Severity.Error);
            return default;
        }
        catch (Exception)
        {
            snackbar.Add(unreachable(), Severity.Error);
            return default;
        }
    }

    private static async Task<bool> RunAsyncCore(Func<Task> action, ISnackbar snackbar, Func<string> unreachable, Action<bool>? setBusy, bool useGlobalBusy)
    {
        if (setBusy is not null)
            setBusy(true);
        if (useGlobalBusy)
            BusyState.Instance.Begin();
        try
        {
            await action();
            return true;
        }
        catch (ApiException ex)
        {
            snackbar.Add(FormatError(ex), Severity.Error);
            return false;
        }
        catch (Exception)
        {
            snackbar.Add(unreachable(), Severity.Error);
            return false;
        }
        finally
        {
            if (setBusy is not null)
                setBusy(false);
            if (useGlobalBusy)
                BusyState.Instance.End();
        }
    }

    internal static string FormatError(ApiException ex)
    {
        if (ex.Error?.Details is JsonElement { ValueKind: JsonValueKind.Array } arr)
        {
            var messages = arr.EnumerateArray()
                .Select(e =>
                {
                    if (e.TryGetProperty("errorMessage", out var msg))
                        return msg.GetString();
                    if (e.TryGetProperty("Message", out var msg2))
                        return msg2.GetString();
                    return null;
                })
                .Where(m => !string.IsNullOrWhiteSpace(m))
                .Distinct()
                .ToList();

            if (messages.Count > 0)
                return string.Join(Environment.NewLine, messages);
        }

        return ex.Message;
    }
}
