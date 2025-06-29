using FinFlow.Application.Contracts.Shared;
using FinFlow.Application.Contracts.Transactions;
using FinFlow.Application.Contracts.Transactions.Request;
using FinFlow.Application.Contracts.Transactions.Response;
using FinFlow.Application.Contracts.Users;
using FinFlow.Application.Implementation.Transactions.Mappers;
using FinFlow.Domain.Repositories;
using FinFlow.Domain.Enum;
using FinFlow.Domain;

namespace FinFlow.Application.Implementation.Transactions;

public class TransferService(ITransactionRepository transactionRepository, 
    IWalletRepository walletRepository, 
    ICategoryRepository categoryRepository, 
    ICurrentUserService currentUserService) : ITransferService
{
    public async Task<CreatedResponse> TransferBetweenWallets(TransferRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = await currentUserService.GetUserIdAsync(cancellationToken);

        if (request.FromWalletId == request.ToWalletId)
            throw new InvalidOperationException("Cannot transfer to the same wallet.");

        if (request.Amount <= 0)
        {
            throw new InvalidOperationException("Transfer amount must be greater than zero.");
        }

        var fromWallet = await walletRepository.Get(request.FromWalletId, cancellationToken);
        if (fromWallet == null || fromWallet.UserId != currentUserId)
        {
            throw new KeyNotFoundException($"Source wallet with id '{request.FromWalletId}' not found.");
        }

        var toWallet = await walletRepository.Get(request.ToWalletId, cancellationToken);
        if (toWallet == null || toWallet.UserId != currentUserId)
        {
            throw new KeyNotFoundException($"Destination wallet with id '{request.ToWalletId}' not found.");
        }

        if (fromWallet.Balance < request.Amount)
        {
            throw new InvalidOperationException($"Insufficient balance in source wallet. Available: {fromWallet.Balance}, Required: {request.Amount}");
        }

        if (fromWallet.Currency != toWallet.Currency)
        {
            if (!request.ReceivedAmount.HasValue)
            {
                throw new InvalidOperationException($"ReceivedAmount is required when transferring between different currencies. From: {fromWallet.Currency}, To: {toWallet.Currency}");
            }

            if (request.ReceivedAmount <= 0)
            {
                throw new InvalidOperationException("ReceivedAmount must be greater than zero when transferring between different currencies.");
            }
        }
        else
        {
            request.ReceivedAmount = null;
        }

        var transferCategory = await GetOrCreateTransferCategory(currentUserId, cancellationToken);

        var fromTransaction = TransferMappers.MapToTransaction(request, currentUserId, transferCategory.Id, true, toWallet.Name, toWallet.Currency.ToString());
        var toTransaction = TransferMappers.MapToTransaction(request, currentUserId, transferCategory.Id, false, fromWallet.Name, fromWallet.Currency.ToString());

        fromWallet.Balance -= request.Amount;
        toWallet.Balance += request.ReceivedAmount ?? request.Amount;

        await walletRepository.Update(fromWallet, cancellationToken);
        await walletRepository.Update(toWallet, cancellationToken);
        await transactionRepository.Create(fromTransaction, cancellationToken);
        await transactionRepository.Create(toTransaction, cancellationToken);

        var originalNotes = TransferMappers.ExtractOriginalNotes(fromTransaction.Notes);
        fromTransaction.Notes = originalNotes != null ? $"{originalNotes}|RelatedTransactionId:{toTransaction.Id}" : $"RelatedTransactionId:{toTransaction.Id}";
        toTransaction.Notes = originalNotes != null ? $"{originalNotes}|RelatedTransactionId:{fromTransaction.Id}" : $"RelatedTransactionId:{fromTransaction.Id}";

        await transactionRepository.Update(fromTransaction, cancellationToken);
        await transactionRepository.Update(toTransaction, cancellationToken);

        return new CreatedResponse(fromTransaction.Id);
    }

    public async Task<TransferResponse> Get(string id, CancellationToken cancellationToken)
    {
        var currentUserId = await currentUserService.GetUserIdAsync(cancellationToken);

        var transaction = await transactionRepository.Get(id, cancellationToken);
        if (transaction == null || transaction.UserId != currentUserId)
        {
            throw new KeyNotFoundException($"Transfer with id '{id}' not found.");
        }

        if (transaction.Type != TransactionType.Transfer)
        {
            throw new InvalidOperationException($"Transaction with id '{id}' is not a transfer.");
        }

        var relatedTransactionId = TransferMappers.ExtractRelatedTransactionId(transaction.Notes);
        Transaction? relatedTransaction = null;

        if (relatedTransactionId != null)
        {
            relatedTransaction = await transactionRepository.Get(relatedTransactionId, cancellationToken);
        }

        if (relatedTransaction == null)
        {
            return TransferMappers.MapFromTransaction(transaction);
        }

        var fromTransaction = transaction.Description.Contains("Transfer sent to") ? transaction : relatedTransaction;
        var toTransaction = transaction.Description.Contains("Transfer received from") ? transaction : relatedTransaction;

        return TransferMappers.MapFromTransactions(fromTransaction, toTransaction);
    }

    public async Task<TransferResponse[]> GetAll(CancellationToken cancellationToken)
    {
        var currentUserId = await currentUserService.GetUserIdAsync(cancellationToken);
        var allTransactions = await transactionRepository.GetAll(cancellationToken);

        var userTransferTransactions = allTransactions
            .Where(t => t.UserId == currentUserId &&
                       t.Type == TransactionType.Transfer)
            .OrderByDescending(t => t.Date)
            .ToList();

        var transfers = new List<TransferResponse>();
        var processedIds = new HashSet<string>();

        foreach (var transaction in userTransferTransactions)
        {
            if (processedIds.Contains(transaction.Id))
            {
                continue;
            }

            var relatedTransactionId = TransferMappers.ExtractRelatedTransactionId(transaction.Notes);

            if (relatedTransactionId != null)
            {
                var relatedTransaction = userTransferTransactions.FirstOrDefault(t => t.Id == relatedTransactionId);
                if (relatedTransaction != null)
                {
                    var fromTransaction = transaction.Description.Contains("Transfer sent to") ? transaction : relatedTransaction;
                    var toTransaction = transaction.Description.Contains("Transfer received from") ? transaction : relatedTransaction;

                    transfers.Add(TransferMappers.MapFromTransactions(fromTransaction, toTransaction));
                    processedIds.Add(transaction.Id);
                    processedIds.Add(relatedTransaction.Id);
                    continue;
                }
            }

            transfers.Add(TransferMappers.MapFromTransaction(transaction));
            processedIds.Add(transaction.Id);
        }

        return transfers.ToArray();
    }

    public async Task Update(string id, UpdateTransferRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = await currentUserService.GetUserIdAsync(cancellationToken);

        if (request.FromWalletId == request.ToWalletId)
        {
            throw new InvalidOperationException("Cannot transfer to the same wallet.");
        }

        if (request.Amount <= 0)
        {
            throw new InvalidOperationException("Transfer amount must be greater than zero.");
        }

        var transaction = await transactionRepository.Get(id, cancellationToken);
        if (transaction == null || transaction.UserId != currentUserId)
        {
            throw new KeyNotFoundException($"Transfer with id '{id}' not found.");
        }

        if (transaction.Type != TransactionType.Transfer)
        {
            throw new InvalidOperationException($"Transaction with id '{id}' is not a transfer.");
        }

        var relatedTransactionId = TransferMappers.ExtractRelatedTransactionId(transaction.Notes);
        Transaction? relatedTransaction = null;

        if (relatedTransactionId != null)
        {
            relatedTransaction = await transactionRepository.Get(relatedTransactionId, cancellationToken);
        }

        var fromWallet = await walletRepository.Get(request.FromWalletId, cancellationToken);
        if (fromWallet == null || fromWallet.UserId != currentUserId)
        {
            throw new KeyNotFoundException($"Source wallet with id '{request.FromWalletId}' not found.");
        }

        var toWallet = await walletRepository.Get(request.ToWalletId, cancellationToken);
        if (toWallet == null || toWallet.UserId != currentUserId)
        {
            throw new KeyNotFoundException($"Destination wallet with id '{request.ToWalletId}' not found.");
        }

        if (fromWallet.Currency != toWallet.Currency)
        {
            if (!request.ReceivedAmount.HasValue)
            {
                throw new InvalidOperationException($"ReceivedAmount is required when transferring between different currencies. From: {fromWallet.Currency}, To: {toWallet.Currency}");
            }

            if (request.ReceivedAmount <= 0)
            {
                throw new InvalidOperationException("ReceivedAmount must be greater than zero when transferring between different currencies.");
            }
        }
        else
        {
            request.ReceivedAmount = null;
        }

        var originalNotes = TransferMappers.ExtractOriginalNotes(transaction.Notes);

        if (relatedTransaction != null)
        {
            var oldFromWallet = await walletRepository.Get(transaction.WalletId, cancellationToken);
            var oldToWallet = await walletRepository.Get(relatedTransaction.WalletId, cancellationToken);

            if (oldFromWallet != null)
            {
                oldFromWallet.Balance += transaction.Amount;
            }

            if (oldToWallet != null)
            {
                oldToWallet.Balance -= relatedTransaction.Amount;
            }

            fromWallet.Balance -= request.Amount;
            toWallet.Balance += request.ReceivedAmount ?? request.Amount;

            transaction.Description = $"Transfer sent to \"{toWallet.Name}\" {toWallet.Currency}";
            transaction.Amount = request.Amount;
            transaction.Notes = originalNotes != null ? $"{originalNotes}|RelatedTransactionId:{relatedTransaction.Id}" : $"RelatedTransactionId:{relatedTransaction.Id}";
            transaction.WalletId = request.FromWalletId;

            relatedTransaction.Description = $"Transfer received from \"{fromWallet.Name}\" {fromWallet.Currency}";
            relatedTransaction.Amount = request.ReceivedAmount ?? request.Amount;
            relatedTransaction.Notes = originalNotes != null ? $"{originalNotes}|RelatedTransactionId:{transaction.Id}" : $"RelatedTransactionId:{transaction.Id}";
            relatedTransaction.WalletId = request.ToWalletId;

            var walletsToUpdate = new List<Domain.Wallet>();

            if (oldFromWallet != null && oldFromWallet.Id != fromWallet.Id)
            {
                walletsToUpdate.Add(oldFromWallet);
            }

            if (oldToWallet != null && oldToWallet.Id != toWallet.Id)
            {
                walletsToUpdate.Add(oldToWallet);
            }

            if (oldFromWallet?.Id != fromWallet.Id)
            {
                walletsToUpdate.Add(fromWallet);
            }

            if (oldToWallet?.Id != toWallet.Id)
            {
                walletsToUpdate.Add(toWallet);
            }
                                
            foreach (var wallet in walletsToUpdate)
            {
                await walletRepository.Update(wallet, cancellationToken);
            }

            await transactionRepository.Update(transaction, cancellationToken);
            await transactionRepository.Update(relatedTransaction, cancellationToken);
        }
        else
        {
            var oldWallet = await walletRepository.Get(transaction.WalletId, cancellationToken);
            if (oldWallet != null)
            {
                if (transaction.Description.Contains("Transfer sent to"))
                {
                    oldWallet.Balance += transaction.Amount;
                }
                else
                {
                    oldWallet.Balance -= transaction.Amount;
                }
            }

            if (transaction.Description.Contains("Transfer sent to"))
            {
                fromWallet.Balance -= request.Amount;
                transaction.Description = $"Transfer sent to \"{toWallet.Name}\" {toWallet.Currency}";
                transaction.Amount = request.Amount;
                transaction.WalletId = request.FromWalletId;
            }
            else
            {
                toWallet.Balance += request.ReceivedAmount ?? request.Amount;
                transaction.Description = $"Transfer received from \"{fromWallet.Name}\" {fromWallet.Currency}";
                transaction.Amount = request.ReceivedAmount ?? request.Amount;
                transaction.WalletId = request.ToWalletId;
            }

            transaction.Notes = originalNotes;

            var walletsToUpdate = new List<Domain.Wallet>();

            if (oldWallet != null && oldWallet.Id != fromWallet.Id && oldWallet.Id != toWallet.Id)
            {
                walletsToUpdate.Add(oldWallet);
            }

            if (oldWallet?.Id != fromWallet.Id)
            {
                walletsToUpdate.Add(fromWallet);
            }

            if (oldWallet?.Id != toWallet.Id)
            {
                walletsToUpdate.Add(toWallet);
            }

            foreach (var wallet in walletsToUpdate)
            {
                await walletRepository.Update(wallet, cancellationToken);
            }

            await transactionRepository.Update(transaction, cancellationToken);
        }
    }

    public async Task Delete(string id, CancellationToken cancellationToken)
    {
        var currentUserId = await currentUserService.GetUserIdAsync(cancellationToken);

        var transaction = await transactionRepository.Get(id, cancellationToken);
        if (transaction == null || transaction.UserId != currentUserId)
        {
            throw new KeyNotFoundException($"Transfer with id '{id}' not found.");
        }

        if (transaction.Type != TransactionType.Transfer)
        {
            throw new InvalidOperationException($"Transaction with id '{id}' is not a transfer.");
        }

        var relatedTransactionId = TransferMappers.ExtractRelatedTransactionId(transaction.Notes);
        Transaction? relatedTransaction = null;

        if (relatedTransactionId != null)
        {
            relatedTransaction = await transactionRepository.Get(relatedTransactionId, cancellationToken);
        }

        if (relatedTransaction != null)
        {
            var fromTransaction = transaction.Description.Contains("Transfer sent to") ? transaction : relatedTransaction;
            var toTransaction = transaction.Description.Contains("Transfer received from") ? transaction : relatedTransaction;

            var fromWallet = await walletRepository.Get(fromTransaction.WalletId, cancellationToken);
            var toWallet = await walletRepository.Get(toTransaction.WalletId, cancellationToken);

            if (fromWallet != null)
            {
                fromWallet.Balance += fromTransaction.Amount;
            }

            if (toWallet != null)
            {
                toWallet.Balance -= toTransaction.Amount;
            }

            if (fromWallet != null)
            {
                await walletRepository.Update(fromWallet, cancellationToken);
            }

            if (toWallet != null)
            {
                await walletRepository.Update(toWallet, cancellationToken);
            }

            await transactionRepository.Delete(transaction.Id, cancellationToken);
            await transactionRepository.Delete(relatedTransaction.Id, cancellationToken);
        }
        else
        {
            var wallet = await walletRepository.Get(transaction.WalletId, cancellationToken);

            if (wallet != null)
            {
                if (transaction.Description.Contains("Transfer sent to"))
                {
                    wallet.Balance += transaction.Amount;
                }
                else
                {
                    wallet.Balance -= transaction.Amount;
                }

                await walletRepository.Update(wallet, cancellationToken);
            }

            await transactionRepository.Delete(transaction.Id, cancellationToken);
        }
    }

    private async Task<Category> GetOrCreateTransferCategory(string userId, CancellationToken cancellationToken)
    {
        var categories = await categoryRepository.GetAll(cancellationToken);
        var transferCategory = categories.FirstOrDefault(c =>
            c.UserId == userId &&
            c.Name.ToLower().Contains("transfer") &&
            c.Type == CategoryType.Expense);

        if (transferCategory != null)
        {
            return transferCategory;
        }

        transferCategory = TransferMappers.MapToTransferCategory(userId);
        await categoryRepository.Create(transferCategory, cancellationToken);
        return transferCategory;
    }
}