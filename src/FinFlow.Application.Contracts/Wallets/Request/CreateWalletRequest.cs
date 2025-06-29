using FinFlow.Domain.Enum;

namespace FinFlow.Application.Contracts.Wallets.Request;

public class CreateWalletRequest
{
    public string Name { get; set; } = string.Empty;
    public WalletType Type { get; set; }
    public decimal Balance { get; set; }
    public CurrencyType Currency { get; set; }
    public string? Description { get; set; }
}