using RadiologyCenter.Idnetity.Domain.Entities;

namespace RadiologyCenter.Idnetity.Application.Abstractions;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}
