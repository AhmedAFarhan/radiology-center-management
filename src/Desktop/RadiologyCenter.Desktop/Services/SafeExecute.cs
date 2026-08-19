using MudBlazor;

namespace RadiologyCenter.Desktop.Services;

public static class SafeExecute
{
    public static Task<bool> RunAsync(Func<Task> action, ISnackbar snackbar, Func<string> unreachable)
        => RunAsyncCore(action, snackbar, unreachable, null, useGlobalBusy: true);

    public static Task<bool> RunAsync(Func<Task> action, ISnackbar snackbar, Func<string> unreachable, Action<bool> setBusy)
        => RunAsyncCore(action, snackbar, unreachable, setBusy, useGlobalBusy: false);

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
            snackbar.Add(ex.Message, Severity.Error);
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
}