namespace FinFlow.Domain.Repositories;

public interface IUserRepository
{
    Task Create(User user, CancellationToken cancellationToken);
    Task<User?> GetUser(Guid id, CancellationToken cancellationToken);
    Task<User?> GetUserByAuthId(string authId, CancellationToken cancellationToken);
}