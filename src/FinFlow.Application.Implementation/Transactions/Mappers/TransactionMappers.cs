using FinFlow.Application.Contracts.Transactions.Request;
using FinFlow.Application.Contracts.Transactions.Response;
using FinFlow.Domain;

namespace FinFlow.Application.Implementation.Transactions.Mappers;

public static class TransactionMappers
{
    internal static Transaction MapFromCreateRequest(CreateTransactionRequest request)
    {
        return new Transaction
        {
            Description = request.Description,
            Amount = request.Amount,
            Type = request.Type,
            Date = request.Date,
            Notes = request.Notes,
            CategoryId = request.CategoryId
        };
    }

    internal static Transaction MapFromUpdateRequest(UpdateTransactionRequest request, Transaction transaction)
    {
        transaction.Description = request.Description;
        transaction.Amount = request.Amount;
        transaction.Type = request.Type;
        transaction.Date = request.Date;
        transaction.Notes = request.Notes;
        transaction.CategoryId = request.CategoryId;

        return transaction;
    }

    internal static TransactionResponse MapFromEntity(Transaction transaction)
    {
        return new TransactionResponse
        {
            Id = transaction.Id,
            Description = transaction.Description,
            Amount = transaction.Amount,
            Type = transaction.Type,
            Date = transaction.Date,
            Notes = transaction.Notes
        };
    }
}