using FinFlow.Domain.Audit;
using FinFlow.Domain.Enum;

namespace FinFlow.Domain;

public class Wallet : Entity
{
    public string Name { get; set; } = string.Empty;
    public WalletType Type { get; set; }
    public decimal Balance { get; set; }
    public CurrencyType Currency { get; set; }
    public string? Description { get; set; }
    public string UserId { get; set; } = string.Empty;
    public User User { get; set; } = null!;
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}