namespace RadiologyCenter.Desktop.Features.Auth.Services;

public static class SignInCompletion
{
    public static async Task CompleteAsync(
        bool passwordChangeRequired,
        Func<Task<bool>> changePassword,
        Func<Task> signOut,
        Action continueToApplication)
    {
        if (passwordChangeRequired && !await changePassword())
        {
            await signOut();
            return;
        }

        continueToApplication();
    }
}
