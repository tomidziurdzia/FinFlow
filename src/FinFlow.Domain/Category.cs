using FinFlow.Domain.Audit;
using FinFlow.Domain.Enum;

namespace FinFlow.Domain;

public class Category : Entity
{
    public string Name { get; set; }
    public CategoryType Type { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
}