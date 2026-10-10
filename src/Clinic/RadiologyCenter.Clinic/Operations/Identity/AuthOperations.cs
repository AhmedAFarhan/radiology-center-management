
using RadiologyCenter.Clinic.Dispatching;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Identity.Application.Commands.Login;
using RadiologyCenter.Identity.Application.Commands.Logout;
using RadiologyCenter.Identity.Application.Commands.RefreshToken;
using RadiologyCenter.Identity.Application.Commands.ChangePassword;
using RadiologyCenter.Identity.Application.DTOs;
using RadiologyCenter.Clinic.Extensions;
using Wolverine;

namespace RadiologyCenter.Clinic.Operations.Identity;

public class AuthOperations : ClinicOperations
{
    private readonly IMessageBus _bus;

    public AuthOperations(IMessageBus bus) => _bus = bus;

    [AllowUnauthenticated]
    [ClinicOperation("POST", "auth/login")]
    public async Task<ClinicResult> LoginAsync([FromBody] LoginCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<TokenResult>>(command, ct);
        return result.ToActionResult();
    }

    [AllowUnauthenticated]
    [ClinicOperation("POST", "auth/refresh")]
    public async Task<ClinicResult> RefreshAsync([FromBody] RefreshTokenCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<TokenResult>>(command, ct);
        return result.ToActionResult();
    }

    [RequireSignIn]
    [ClinicOperation("POST", "auth/logout")]
    public async Task<ClinicResult> LogoutAsync([FromBody] LogoutCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command, ct);
        return result.ToActionResult();
    }

    [RequireSignIn]
    [ClinicOperation("POST", "auth/change-password")]
    public async Task<ClinicResult> ChangePasswordAsync([FromBody] ChangePasswordCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<TokenResult>>(command, ct);
        return result.ToActionResult();
    }
}
