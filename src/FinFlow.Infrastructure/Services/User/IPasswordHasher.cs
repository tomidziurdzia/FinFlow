using FinFlow.Application.Contracts.Users;
using BC = BCrypt.Net.BCrypt;

namespace FinFlow.Infrastructure.Services.User;

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        return BC.HashPassword(password);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        return BC.Verify(password, hashedPassword);
    }
}
