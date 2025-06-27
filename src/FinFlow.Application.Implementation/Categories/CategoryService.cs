using FinFlow.Application.Contracts.Categories;
using FinFlow.Application.Contracts.Categories.Request;
using FinFlow.Application.Contracts.Categories.Response;
using FinFlow.Application.Contracts.Shared;

namespace FinFlow.Application.Implementation.Categories;

public class CategoryService : ICategoryService
{
    public Task<CategoryResponse> Get(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<CategoryResponse[]> GetAll(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<CreatedResponse> Create(CategoryRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task Update(CategoryRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task Delete(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}