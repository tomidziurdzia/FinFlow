using FinFlow.Domain.Audit;
using Microsoft.AspNetCore.Identity;

namespace FinFlow.Domain;

public class User : IdentityUser, IAudit
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public string CreatedBy { get; set; }
    public string ModifiedBy { get; set; }
}