using FinFlow.Domain;

namespace FinFlow.Domain.Repositories;

public interface ITransactionRepository
{
    Task<Transaction?> Get(string id, CancellationToken cancellationToken);
    Task<Transaction[]> GetAll(CancellationToken cancellationToken);
    Task Create(Transaction transaction, CancellationToken cancellationToken);
    Task Update(Transaction transaction, CancellationToken cancellationToken);
    Task Delete(string id, CancellationToken cancellationToken);
}