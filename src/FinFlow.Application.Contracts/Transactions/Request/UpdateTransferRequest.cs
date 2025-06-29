namespace FinFlow.Application.Contracts.Transactions.Request;

public class UpdateTransferRequest
{
    public decimal Amount { get; set; }
    public decimal? ReceivedAmount { get; set; }
    public string FromWalletId { get; set; } = string.Empty;
    public string ToWalletId { get; set; } = string.Empty;
}