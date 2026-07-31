using RadiologyCenter.Identity.Application.DTOs;
using RadiologyCenter.Identity.Domain.Entities;

namespace RadiologyCenter.Identity.Application.Abstractions;

public interface ITokenService
{
    TokenResult GenerateTokenResult(User user);
}
