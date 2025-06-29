using FinFlow.Domain.Enum;

namespace FinFlow.Application.Contracts.Transactions.Request;

public class UpdateTransactionRequest
{
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public DateTime Date { get; set; }
    public string? Notes { get; set; }
    public string CategoryId { get; set; } = string.Empty;
    public string WalletId { get; set; } = string.Empty;
}
