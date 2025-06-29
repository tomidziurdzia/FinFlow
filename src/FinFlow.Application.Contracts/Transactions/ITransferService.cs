using FinFlow.Application.Contracts.Shared;
using FinFlow.Application.Contracts.Transactions.Request;
using FinFlow.Application.Contracts.Transactions.Response;

namespace FinFlow.Application.Contracts.Transactions;

public interface ITransferService
{
    Task<CreatedResponse> TransferBetweenWallets(TransferRequest request, CancellationToken cancellationToken);
    Task<TransferResponse> Get(string id, CancellationToken cancellationToken);
    Task<TransferResponse[]> GetAll(CancellationToken cancellationToken);
    Task Update(string id, UpdateTransferRequest request, CancellationToken cancellationToken);
    Task Delete(string id, CancellationToken cancellationToken);
}