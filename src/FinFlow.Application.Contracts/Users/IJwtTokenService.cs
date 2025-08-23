using FinFlow.Application.Contracts.Users.Response;

namespace FinFlow.Application.Contracts.Users;

public interface IJwtTokenService
{
    string GenerateToken(UserResponse user);
    bool ValidateToken(string token);
}
