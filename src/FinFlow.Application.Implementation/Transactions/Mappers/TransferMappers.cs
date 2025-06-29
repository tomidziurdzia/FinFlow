using FinFlow.Application.Contracts.Transactions.Request;
using FinFlow.Domain;
using FinFlow.Domain.Enum;

namespace FinFlow.Application.Implementation.Transactions.Mappers;

public static class TransferMappers
{
    internal static Transaction MapToFromTransaction(TransferRequest request, string userId, string categoryId)
    {
        return new Transaction
        {
            Id = Guid.NewGuid().ToString(),
            Description = $"Transfer out: {request.Description}",
            Amount = request.Amount,
            Type = TransactionType.Transfer,
            Date = DateTime.UtcNow,
            Notes = request.Notes,
            UserId = userId,
            CategoryId = categoryId,
            WalletId = request.FromWalletId
        };
    }

    internal static Transaction MapToToTransaction(TransferRequest request, string userId, string categoryId)
    {
        return new Transaction
        {
            Id = Guid.NewGuid().ToString(),
            Description = $"Transfer in: {request.Description}",
            Amount = request.ReceivedAmount ?? request.Amount,
            Type = TransactionType.Transfer,
            Date = DateTime.UtcNow,
            Notes = request.Notes,
            UserId = userId,
            CategoryId = categoryId,
            WalletId = request.ToWalletId
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
            Icon = "🔄",
            UserId = userId
        };
    }
}