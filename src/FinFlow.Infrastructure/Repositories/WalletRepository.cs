using FinFlow.Domain;
using FinFlow.Domain.Repositories;
using FinFlow.Infrastructure.Exceptions;
using FinFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace FinFlow.Infrastructure.Repositories;

public class WalletRepository(ApplicationDbContext applicationDbContext) : IWalletRepository
{
    public Task<Wallet?> Get(string id, CancellationToken cancellationToken)
    {
        return applicationDbContext.Wallets.AsNoTracking().FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    public Task<Wallet[]> GetAll(CancellationToken cancellationToken)
    {
        return applicationDbContext.Wallets.AsNoTracking().ToArrayAsync(cancellationToken);
    }

    public async Task Create(Wallet wallet, CancellationToken cancellationToken)
    {
        try
        {
            await applicationDbContext.Wallets.AddAsync(wallet, cancellationToken);
            await applicationDbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            throw new EntityAlreadyExistsException($"A wallet with id '{wallet.Id}' already exists.");
        }
    }

    public Task Update(Wallet wallet, CancellationToken cancellationToken)
    {
        try
        {
            applicationDbContext.Wallets.Update(wallet);
            return applicationDbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new EntityNotFoundException($"Wallet with id '{wallet.Id}' not found.");
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            throw new EntityAlreadyExistsException($"A wallet with id '{wallet.Id}' already exists.");
        }
    }

    public async Task Delete(string id, CancellationToken cancellationToken)
    {
        var wallet = await applicationDbContext.Wallets.FindAsync(new object[] { id }, cancellationToken);
        if (wallet == null)
        {
            throw new EntityNotFoundException($"Wallet with id '{id}' not found.");
        }

        applicationDbContext.Wallets.Remove(wallet);
        await applicationDbContext.SaveChangesAsync(cancellationToken);
    }
}