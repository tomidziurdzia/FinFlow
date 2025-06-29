using FinFlow.Domain.Enum;

namespace FinFlow.Application.Contracts.Transactions.Response;

public class TransactionResponse
{
    public string Id { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public DateTime Date { get; set; }
    public string? Notes { get; set; }
}