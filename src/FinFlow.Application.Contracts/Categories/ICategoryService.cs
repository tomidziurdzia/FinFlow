using FinFlow.Application.Contracts.Categories.Request;
using FinFlow.Application.Contracts.Categories.Response;
using FinFlow.Application.Contracts.Shared;

namespace FinFlow.Application.Contracts.Categories;

public interface ICategoryService
{
    Task<CategoryResponse> Get(string id, CancellationToken cancellationToken);
    Task<CategoryResponse[]> GetAll(CancellationToken cancellationToken);
    Task<CreatedResponse> Create(CategoryRequest request, CancellationToken cancellationToken);
    Task Update(string id, CategoryRequest request, CancellationToken cancellationToken);
    Task Delete(string id, CancellationToken cancellationToken);
}