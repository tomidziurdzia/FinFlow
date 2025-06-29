using FinFlow.Domain;

namespace FinFlow.Domain.Repositories;

public interface IWalletRepository
{
    Task<Wallet?> Get(string id, CancellationToken cancellationToken);
    Task<Wallet[]> GetAll(CancellationToken cancellationToken);
    Task Create(Wallet wallet, CancellationToken cancellationToken);
    Task Update(Wallet wallet, CancellationToken cancellationToken);
    Task Delete(string id, CancellationToken cancellationToken);
}