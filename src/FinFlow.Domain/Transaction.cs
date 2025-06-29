using FinFlow.Domain.Audit;
using FinFlow.Domain.Enum;

namespace FinFlow.Domain;

public class Transaction : Entity
{
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public DateTime Date { get; set; }
    public string? Notes { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string CategoryId { get; set; } = string.Empty;
    public User User { get; set; } = null!;
    public Category Category { get; set; } = null!;
}