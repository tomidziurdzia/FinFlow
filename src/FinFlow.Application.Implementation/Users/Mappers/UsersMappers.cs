using FinFlow.Application.Contracts.Users.Request;
using FinFlow.Application.Contracts.Users.Response;
using FinFlow.Domain;

namespace FinFlow.Application.Implementation.Users.Mappers;

internal static class UsersMappers
{
    internal static User MapFromCreateRequest(UserRequest request)
    {
        return new User
        {
            Id = new Guid(),
            AuthId = request.AuthId,
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
            FirstName: user.FirstName,
            LastName: user.LastName
        );
    }
}