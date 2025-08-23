namespace FinFlow.Domain.Repositories;

public interface IUserRepository
{
    Task Create(User user, CancellationToken cancellationToken);
    Task<User?> GetUser(string id, CancellationToken cancellationToken);
    Task<User?> GetUserByAuthId(string authId, CancellationToken cancellationToken);
    Task<User?> GetUserByEmail(string email, CancellationToken cancellationToken);
    Task<string> GetUserIdByAuthId(string authId, CancellationToken cancellationToken);
}