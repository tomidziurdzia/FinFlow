using FinFlow.Domain;
using FinFlow.Domain.Repositories;

namespace FinFlow.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    public Task<Category?> Get(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Category[]> GetAll(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task Create(Category category, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task Update(Category category, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task Delete(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}