using FinFlow.Domain;
using FinFlow.Domain.Repositories;
using FinFlow.Infrastructure.Exceptions;
using FinFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace FinFlow.Infrastructure.Repositories;

public class UserRepository(ApplicationDbContext applicationDbContext) : IUserRepository
{
    public async Task Create(User user, CancellationToken cancellationToken)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        try
        {
            await applicationDbContext.Users.AddAsync(user, cancellationToken);
            await applicationDbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            throw new EntityAlreadyExistsException($"A user with AuthId '{user.AuthId}' already exists.");
        }
    }

    public async Task<User?> GetUser(string id, CancellationToken cancellationToken)
    {
        return await applicationDbContext.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetUserByAuthId(string authId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(authId))
        {
            throw new ArgumentException("AuthId is required.", nameof(authId));
        }

        return await applicationDbContext.Users
            .FirstOrDefaultAsync(u => u.AuthId == authId, cancellationToken);
    }

    public async Task<string> GetUserIdByAuthId(string authId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(authId))
        {
            throw new ArgumentException("AuthId is required.", nameof(authId));
        }

        var user = await applicationDbContext.Users.FirstOrDefaultAsync(u => u.AuthId == authId, cancellationToken)
            ?? throw new ArgumentException($"User with AuthId '{authId}' not found.");

        return user.Id;
    }
}