using FinFlow.Application.Contracts.Users.Request;
using FinFlow.Application.Contracts.Users.Response;

namespace FinFlow.Application.Contracts.Users;

public interface IUserService
{
    Task<UserResponse> Create(UserRequest request, CancellationToken cancellationToken);
    Task<UserResponse?> GetUser(string id, CancellationToken cancellationToken);
    Task<UserResponse?> GetUserByAuthId(UserRequest request, CancellationToken cancellationToken);
}