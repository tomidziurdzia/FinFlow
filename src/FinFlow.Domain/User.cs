using FinFlow.Domain.Audit;

namespace FinFlow.Domain;

public class User : Entity
{
    public string AuthId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public ICollection<Category> Categories { get; set; }
    public ICollection<Transaction> Transactions { get; set; }
}