using FinFlow.Domain;
using FinFlow.Domain.Repositories;
using FinFlow.Infrastructure.Exceptions;
using FinFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace FinFlow.Infrastructure.Repositories;

public class TransactionRepository(ApplicationDbContext applicationDbContext) : ITransactionRepository
{
    public Task<Transaction?> Get(string id, CancellationToken cancellationToken)
    {
        return applicationDbContext.Transactions.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public Task<Transaction[]> GetAll(CancellationToken cancellationToken)
    {
        return applicationDbContext.Transactions.AsNoTracking().ToArrayAsync(cancellationToken);
    }

    public async Task Create(Transaction transaction, CancellationToken cancellationToken)
    {
        try
        {
            await applicationDbContext.Transactions.AddAsync(transaction, cancellationToken);
            await applicationDbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            throw new EntityAlreadyExistsException($"A transaction with id '{transaction.Id}' already exists.");
        }
    }

    public Task Update(Transaction transaction, CancellationToken cancellationToken)
    {
        try
        {
            applicationDbContext.Transactions.Update(transaction);
            return applicationDbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new EntityNotFoundException($"Transaction with id '{transaction.Id}' not found.");
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            throw new EntityAlreadyExistsException($"A transaction with id '{transaction.Id}' already exists.");
        }
    }

    public async Task Delete(string id, CancellationToken cancellationToken)
    {
        var transaction = await applicationDbContext.Transactions.FindAsync(new object[] { id }, cancellationToken);
        if (transaction == null)
        {
            throw new EntityNotFoundException($"Transaction with id '{id}' not found.");
        }

        applicationDbContext.Transactions.Remove(transaction);
        await applicationDbContext.SaveChangesAsync(cancellationToken);
    }
}