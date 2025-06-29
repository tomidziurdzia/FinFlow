using FinFlow.Domain;
using FinFlow.Domain.Repositories;
using FinFlow.Infrastructure.Exceptions;
using FinFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace FinFlow.Infrastructure.Repositories;

public class CategoryRepository(ApplicationDbContext applicationDbContext) : ICategoryRepository
{
    public Task<Category?> Get(string id, CancellationToken cancellationToken)
    {
        return applicationDbContext.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public Task<Category[]> GetAll(CancellationToken cancellationToken)
    {
        return applicationDbContext.Categories.AsNoTracking().ToArrayAsync(cancellationToken);
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
        try
        {
            applicationDbContext.Categories.Update(category);
            return applicationDbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new EntityNotFoundException($"Category with id '{category.Id}' not found.");
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            throw new EntityAlreadyExistsException($"A category with id '{category.Id}' already exists.");
        }
    }

    public async Task Delete(string id, CancellationToken cancellationToken)
    {
        var categories = await applicationDbContext.Categories.ToListAsync(cancellationToken);
        if (categories.Count == 0)
        {
            throw new EntityNotFoundException("No categories found to delete.");
        }

        applicationDbContext.Categories.RemoveRange(categories);
        await applicationDbContext.SaveChangesAsync(cancellationToken);
    }
}