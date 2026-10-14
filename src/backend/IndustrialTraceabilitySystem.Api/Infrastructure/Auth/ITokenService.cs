using IndustrialTraceabilitySystem.Api.Domain.Entities;

namespace IndustrialTraceabilitySystem.Api.Infrastructure.Auth;

public interface ITokenService
{
    (string token, DateTime expiresAt) GenerateToken(User user, IEnumerable<string> roles);
}