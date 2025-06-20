using FinFlow.Domain;
using Microsoft.EntityFrameworkCore;

namespace FinFlow.Application.Contracts.Persistence;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}