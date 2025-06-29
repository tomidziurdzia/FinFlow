using FinFlow.Domain.Enum;

namespace FinFlow.Application.Contracts.Wallets.Response;

public class WalletResponse
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public WalletType Type { get; set; }
    public decimal Balance { get; set; }
    public CurrencyType Currency { get; set; }
    public string? Description { get; set; }
}