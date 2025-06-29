using FinFlow.Application.Contracts.Transactions.Request;
using FinFlow.Application.Contracts.Transactions.Response;
using FinFlow.Domain;
using FinFlow.Domain.Enum;

namespace FinFlow.Application.Implementation.Transactions.Mappers;

public static class TransferMappers
{
    internal static Transaction MapToTransaction(TransferRequest request, string userId, string categoryId, bool isOutgoing, string otherWalletName, string otherWalletCurrency, string? relatedTransactionId = null)
    {
        var description = isOutgoing
            ? $"Transfer sent to \"{otherWalletName}\" {otherWalletCurrency}"
            : $"Transfer received from \"{otherWalletName}\" {otherWalletCurrency}";

        var notes = request.Notes ?? "";
        if (relatedTransactionId != null)
        {
            notes = string.IsNullOrEmpty(notes)
                ? $"RelatedTransactionId:{relatedTransactionId}"
                : $"{notes}|RelatedTransactionId:{relatedTransactionId}";
        }

        return new Transaction
        {
            Id = Guid.NewGuid().ToString(),
            Description = description,
            Amount = isOutgoing ? request.Amount : (request.ReceivedAmount ?? request.Amount),
            Type = TransactionType.Transfer,
            Date = DateTime.UtcNow,
            Notes = notes,
            UserId = userId,
            CategoryId = categoryId,
            WalletId = isOutgoing ? request.FromWalletId : request.ToWalletId
        };
    }

    internal static Category MapToTransferCategory(string userId)
    {
        return new Category
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Transfer",
            Type = CategoryType.Expense,
            Color = "#6B7280",
            Icon = "RotateCcw",
            UserId = userId
        };
    }

    internal static TransferResponse MapFromTransactions(Transaction fromTransaction, Transaction toTransaction)
    {
        return new TransferResponse
        {
            Id = fromTransaction.Id,
            Description = fromTransaction.Description,
            Amount = fromTransaction.Amount,
            FromWalletId = fromTransaction.WalletId,
            ToWalletId = toTransaction.WalletId,
            Notes = fromTransaction.Notes,
            Date = fromTransaction.Date
        };
    }

    internal static TransferResponse MapFromTransaction(Transaction transaction)
    {
        return new TransferResponse
        {
            Id = transaction.Id,
            Description = transaction.Description,
            Amount = transaction.Amount,
            FromWalletId = transaction.WalletId,
            ToWalletId = transaction.WalletId,
            Notes = transaction.Notes,
            Date = transaction.Date
        };
    }

    internal static string? ExtractRelatedTransactionId(string? notes)
    {
        if (string.IsNullOrEmpty(notes))
            return null;

        var parts = notes.Split('|');
        var relatedIdPart = parts.FirstOrDefault(p => p.StartsWith("RelatedTransactionId:"));

        return relatedIdPart?.Replace("RelatedTransactionId:", "");
    }

    internal static string? ExtractOriginalNotes(string? notes)
    {
        if (string.IsNullOrEmpty(notes))
            return null;

        var parts = notes.Split('|');
        var originalNotes = parts.Where(p => !p.StartsWith("RelatedTransactionId:"));

        return originalNotes.Any() ? string.Join("|", originalNotes) : null;
    }
}