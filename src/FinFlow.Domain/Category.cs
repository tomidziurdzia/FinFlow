using FinFlow.Domain.Audit;
using FinFlow.Domain.Enum;

namespace FinFlow.Domain;

public class Category : Entity
{
    public string Name { get; set; } = string.Empty;
    public CategoryType Type { get; set; }
    public string Color { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public User User { get; set; } = null!;
    public ICollection<Transaction> Transactions { get; set; }
}