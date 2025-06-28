using FinFlow.Domain;
using FinFlow.Domain.Repositories;
using FinFlow.Infrastructure.Exceptions;
using FinFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace FinFlow.Infrastructure.Repositories;

public class CategoryRepository(ApplicationDbContext applicationDbContext) : ICategoryRepository
{
    public Task<Category?> Get(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Category[]> GetAll(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task Create(Category category, CancellationToken cancellationToken)
    {
        try
        {
            await applicationDbContext.Categories.AddAsync(category, cancellationToken);
            await applicationDbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            throw new EntityAlreadyExistsException($"A category with id '{category.Id}' already exists.");
        }
    }

    public Task Update(Category category, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task Delete(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}