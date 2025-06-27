namespace FinFlow.Domain.Repositories;

public interface ICategoryRepository
{
    Task<Category?> Get(Guid id, CancellationToken cancellationToken);
    Task<Category[]> GetAll(CancellationToken cancellationToken);
    Task Create(Category category, CancellationToken cancellationToken);
    Task Update(Category category, CancellationToken cancellationToken);
    Task Delete(Guid id, CancellationToken cancellationToken);
}