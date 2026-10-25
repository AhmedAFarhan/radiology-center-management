using RadiologyCenter.Desktop.Features.Auth.Services;
using Xunit;

public sealed class SignInCompletionTests
{
    [Fact]
    public async Task InitialSignInWaitsForSuccessfulPasswordChangeBeforeNavigating()
    {
        var passwordChange = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var navigated = false;
        var signedOut = false;

        var completion = SignInCompletion.CompleteAsync(true,
            () => passwordChange.Task,
            () => { signedOut = true; return Task.CompletedTask; },
            () => navigated = true);

        Assert.False(completion.IsCompleted);
        Assert.False(navigated);
        Assert.False(signedOut);

        passwordChange.SetResult(true);
        await completion;

        Assert.True(navigated);
        Assert.False(signedOut);
    }

    [Fact]
    public async Task InterruptedPasswordChangeSignsOutBeforeCompletingAndDoesNotNavigate()
    {
        var signOut = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var navigated = false;
        var signedOut = false;

        var completion = SignInCompletion.CompleteAsync(true,
            () => Task.FromResult(false),
            () => { signedOut = true; return signOut.Task; },
            () => navigated = true);

        Assert.True(signedOut);
        Assert.False(completion.IsCompleted);
        Assert.False(navigated);

        signOut.SetResult();
        await completion;
        Assert.False(navigated);
    }

    [Fact]
    public async Task ExistingSignInContinuesWithoutRequestingAnotherPasswordChange()
    {
        var navigated = false;

        await SignInCompletion.CompleteAsync(false,
            () => throw new InvalidOperationException("Existing accounts do not need this dialog."),
            () => throw new InvalidOperationException("The signed-in account must be retained."),
            () => navigated = true);

        Assert.True(navigated);
    }
}
