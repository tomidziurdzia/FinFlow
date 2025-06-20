using FinFlow.Domain.Audit;

namespace FinFlow.Domain;

public class User : IAudit
{
    public Guid Id { get; set; }
    public string AuthId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public string CreatedBy { get; set; }
    public string ModifiedBy { get; set; }
}