namespace FinFlow.Application.Contracts.Transactions.Response;

public class TransferResponse
{
    public string Id { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string FromWalletId { get; set; } = string.Empty;
    public string ToWalletId { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime Date { get; set; }
}