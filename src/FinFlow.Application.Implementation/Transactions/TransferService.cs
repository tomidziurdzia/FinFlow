using FinFlow.Application.Contracts.Shared;
using FinFlow.Application.Contracts.Transactions;
using FinFlow.Application.Contracts.Transactions.Request;
using FinFlow.Application.Contracts.Users;
using FinFlow.Application.Implementation.Transactions.Mappers;
using FinFlow.Domain.Repositories;
using FinFlow.Domain.Enum;

namespace FinFlow.Application.Implementation.Transactions;

public class TransferService(ITransactionRepository transactionRepository, IWalletRepository walletRepository, ICategoryRepository categoryRepository, ICurrentUserService currentUserService) : ITransferService
{
    public async Task<CreatedResponse> TransferBetweenWallets(TransferRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = await currentUserService.GetUserIdAsync(cancellationToken);

        if (request.FromWalletId == request.ToWalletId)
            throw new InvalidOperationException("Cannot transfer to the same wallet.");

        if (request.Amount <= 0)
            throw new InvalidOperationException("Transfer amount must be greater than zero.");

        var fromWallet = await walletRepository.Get(request.FromWalletId, cancellationToken);
        if (fromWallet == null || fromWallet.UserId != currentUserId)
            throw new KeyNotFoundException($"Source wallet with id '{request.FromWalletId}' not found.");

        var toWallet = await walletRepository.Get(request.ToWalletId, cancellationToken);
        if (toWallet == null || toWallet.UserId != currentUserId)
            throw new KeyNotFoundException($"Destination wallet with id '{request.ToWalletId}' not found.");

        if (fromWallet.Balance < request.Amount)
            throw new InvalidOperationException($"Insufficient balance in source wallet. Available: {fromWallet.Balance}, Required: {request.Amount}");

        // Validate currency conversion
        if (fromWallet.Currency != toWallet.Currency)
        {
            if (!request.ReceivedAmount.HasValue)
                throw new InvalidOperationException($"ReceivedAmount is required when transferring between different currencies. From: {fromWallet.Currency}, To: {toWallet.Currency}");

            if (request.ReceivedAmount <= 0)
                throw new InvalidOperationException("ReceivedAmount must be greater than zero when transferring between different currencies.");
        }
        else
        {
            // Same currency, ensure ReceivedAmount is not provided or matches the amount
            if (request.ReceivedAmount.HasValue && request.ReceivedAmount != request.Amount)
                throw new InvalidOperationException("ReceivedAmount should not be provided or should match Amount when transferring between wallets with the same currency.");
        }

        var transferCategory = await GetOrCreateTransferCategory(currentUserId, cancellationToken);

        var fromTransaction = TransferMappers.MapToFromTransaction(request, currentUserId, transferCategory.Id);
        var toTransaction = TransferMappers.MapToToTransaction(request, currentUserId, transferCategory.Id);

        fromWallet.Balance -= request.Amount;
        toWallet.Balance += request.ReceivedAmount ?? request.Amount;

        await walletRepository.Update(fromWallet, cancellationToken);
        await walletRepository.Update(toWallet, cancellationToken);
        await transactionRepository.Create(fromTransaction, cancellationToken);
        await transactionRepository.Create(toTransaction, cancellationToken);

        return new CreatedResponse(fromTransaction.Id);
    }

    private async Task<Domain.Category> GetOrCreateTransferCategory(string userId, CancellationToken cancellationToken)
    {
        var categories = await categoryRepository.GetAll(cancellationToken);
        var transferCategory = categories.FirstOrDefault(c =>
            c.UserId == userId &&
            c.Name.ToLower().Contains("transfer") &&
            c.Type == CategoryType.Expense);

        if (transferCategory != null)
            return transferCategory;

        transferCategory = TransferMappers.MapToTransferCategory(userId);
        await categoryRepository.Create(transferCategory, cancellationToken);
        return transferCategory;
    }
}