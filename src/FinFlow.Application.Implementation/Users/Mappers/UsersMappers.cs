using FinFlow.Application.Contracts.Users;
using FinFlow.Application.Contracts.Users.Request;
using FinFlow.Application.Contracts.Users.Response;
using FinFlow.Domain;

namespace FinFlow.Application.Implementation.Users.Mappers;

internal static class UsersMappers
{
    internal static User MapToDomain(UserRequest request)
    {
        return new User
        {
            Id = Guid.NewGuid().ToString(),
            AuthId = request.AuthId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            ModifiedBy = ""
        };
    }

    internal static User MapToDomain(CreateUserRequest request, IPasswordHasher passwordHasher)
    {
        var id = Guid.NewGuid().ToString();
        return new User
        {
            Id = id,
            AuthId = id,
            Email = request.Email,
            Password = passwordHasher.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            ModifiedBy = ""
        };
    }

    internal static UserResponse MapFromEntity(User user)
    {
        return new UserResponse(
            Id: user.Id,
            AuthId: user.AuthId,
            Email: user.Email,
            FirstName: user.FirstName,
            LastName: user.LastName
        );
    }
}