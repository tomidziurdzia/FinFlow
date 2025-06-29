using FinFlow.Application.Contracts.Shared;
using FinFlow.Application.Contracts.Wallets.Request;
using FinFlow.Application.Contracts.Wallets.Response;

namespace FinFlow.Application.Contracts.Wallets;

public interface IWalletService
{
    Task<WalletResponse> Get(string id, CancellationToken cancellationToken);
    Task<WalletResponse[]> GetAll(CancellationToken cancellationToken);
    Task<CreatedResponse> Create(CreateWalletRequest request, CancellationToken cancellationToken);
    Task Update(string id, UpdateWalletRequest request, CancellationToken cancellationToken);
    Task Delete(string id, CancellationToken cancellationToken);
}