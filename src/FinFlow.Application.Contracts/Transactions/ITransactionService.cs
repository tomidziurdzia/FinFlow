using FinFlow.Application.Contracts.Shared;
using FinFlow.Application.Contracts.Transactions.Request;
using FinFlow.Application.Contracts.Transactions.Response;

namespace FinFlow.Application.Contracts.Transactions;

public interface ITransactionService
{
    Task<TransactionResponse> Get(string id, CancellationToken cancellationToken);
    Task<TransactionResponse[]> GetAll(CancellationToken cancellationToken);
    Task<CreatedResponse> Create(CreateTransactionRequest request, CancellationToken cancellationToken);
    Task Update(string id, UpdateTransactionRequest request, CancellationToken cancellationToken);
    Task Delete(string id, CancellationToken cancellationToken);
}