namespace FinFlow.Domain.Audit;

public abstract class Entity : IEntity
{
    public string Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public string CreatedBy { get; set; }
    public string ModifiedBy { get; set; }
}