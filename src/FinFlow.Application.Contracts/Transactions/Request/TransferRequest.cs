namespace FinFlow.Application.Contracts.Transactions.Request;

public class TransferRequest
{
    public string FromWalletId { get; set; } = string.Empty;
    public string ToWalletId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal? ReceivedAmount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Notes { get; set; }
}