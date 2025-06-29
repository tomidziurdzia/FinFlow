using FinFlow.Application.Contracts.Shared;
using FinFlow.Application.Contracts.Transactions.Request;

namespace FinFlow.Application.Contracts.Transactions;

public interface ITransferService
{
    Task<CreatedResponse> TransferBetweenWallets(TransferRequest request, CancellationToken cancellationToken);
}