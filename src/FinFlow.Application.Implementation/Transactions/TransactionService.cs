using FinFlow.Application.Contracts.Shared;
using FinFlow.Application.Contracts.Transactions;
using FinFlow.Application.Contracts.Transactions.Request;
using FinFlow.Application.Contracts.Transactions.Response;
using FinFlow.Application.Contracts.Users;
using FinFlow.Application.Implementation.Transactions.Mappers;
using FinFlow.Domain.Repositories;

namespace FinFlow.Application.Implementation.Transactions;

public class TransactionService(ITransactionRepository transactionRepository, ICategoryRepository categoryRepository, IWalletRepository walletRepository, ICurrentUserService currentUserService) : ITransactionService
{
    public async Task<TransactionResponse> Get(string id, CancellationToken cancellationToken)
    {
        var transaction = await transactionRepository.Get(id, cancellationToken);
        if (transaction == null)
        {
            throw new KeyNotFoundException($"Transaction with id '{id}' not found.");
        }

        var currentUserId = await currentUserService.GetUserIdAsync(cancellationToken);
        if (transaction.UserId != currentUserId)
            throw new UnauthorizedAccessException("You can only access your own transactions.");

        return TransactionMappers.MapFromEntity(transaction);
    }

    public async Task<TransactionResponse[]> GetAll(CancellationToken cancellationToken)
    {
        var currentUserId = await currentUserService.GetUserIdAsync(cancellationToken);
        var transactions = await transactionRepository.GetAll(cancellationToken);

        var userTransactions = transactions.Where(t => t.UserId == currentUserId);

        return userTransactions.Select(TransactionMappers.MapFromEntity).ToArray();
    }

    public async Task<CreatedResponse> Create(CreateTransactionRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = await currentUserService.GetUserIdAsync(cancellationToken);

        var category = await categoryRepository.Get(request.CategoryId, cancellationToken);
        if (category == null || category.UserId != currentUserId)
            throw new KeyNotFoundException($"Category with id '{request.CategoryId}' not found.");

        var wallet = await walletRepository.Get(request.WalletId, cancellationToken);
        if (wallet == null || wallet.UserId != currentUserId)
            throw new KeyNotFoundException($"Wallet with id '{request.WalletId}' not found.");

        var transaction = TransactionMappers.MapFromCreateRequest(request, currentUserId);
        transaction.Id = Guid.NewGuid().ToString();

        await transactionRepository.Create(transaction, cancellationToken);
        return new CreatedResponse(transaction.Id);
    }

    public async Task Update(string id, UpdateTransactionRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = await currentUserService.GetUserIdAsync(cancellationToken);

        var transaction = await transactionRepository.Get(id, cancellationToken);
        if (transaction == null)
            throw new KeyNotFoundException($"Transaction with id '{id}' not found.");

        if (transaction.UserId != currentUserId)
            throw new UnauthorizedAccessException("You can only update your own transactions.");

        var category = await categoryRepository.Get(request.CategoryId, cancellationToken);
        if (category == null || category.UserId != currentUserId)
            throw new KeyNotFoundException($"Category with id '{request.CategoryId}' not found.");

        var wallet = await walletRepository.Get(request.WalletId, cancellationToken);
        if (wallet == null || wallet.UserId != currentUserId)
            throw new KeyNotFoundException($"Wallet with id '{request.WalletId}' not found.");

        TransactionMappers.MapFromUpdateRequest(request, transaction);
        await transactionRepository.Update(transaction, cancellationToken);
    }

    public async Task Delete(string id, CancellationToken cancellationToken)
    {
        var currentUserId = await currentUserService.GetUserIdAsync(cancellationToken);

        var transaction = await transactionRepository.Get(id, cancellationToken);
        if (transaction == null)
            throw new KeyNotFoundException($"Transaction with id '{id}' not found.");

        if (transaction.UserId != currentUserId)
            throw new UnauthorizedAccessException("You can only delete your own transactions.");

        await transactionRepository.Delete(id, cancellationToken);
    }
}