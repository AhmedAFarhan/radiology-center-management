using Microsoft.AspNetCore.Mvc;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Identity.Application.Commands.Login;
using RadiologyCenter.Identity.Application.Commands.Logout;
using RadiologyCenter.Identity.Application.Commands.RefreshToken;
using RadiologyCenter.Identity.Application.DTOs;
using RadiologyCenter.Localhost.Extensions;
using Wolverine;

namespace RadiologyCenter.Localhost.Controllers.Identity;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMessageBus _bus;

    public AuthController(IMessageBus bus) => _bus = bus;

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<TokenResult>>(command, ct);
        return result.ToActionResult();
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshAsync([FromBody] RefreshTokenCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<TokenResult>>(command, ct);
        return result.ToActionResult();
    }

    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync([FromBody] LogoutCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command, ct);
        return result.ToActionResult();
    }
}
