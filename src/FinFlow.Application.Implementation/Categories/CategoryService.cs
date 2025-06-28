using FinFlow.Application.Contracts.Categories;
using FinFlow.Application.Contracts.Categories.Request;
using FinFlow.Application.Contracts.Categories.Response;
using FinFlow.Application.Contracts.Shared;
using FinFlow.Application.Contracts.Users;
using FinFlow.Application.Implementation.Categories.Mappers;
using FinFlow.Domain.Repositories;

namespace FinFlow.Application.Implementation.Categories;

public class CategoryService(ICategoryRepository categoryRepository, ICurrentUserService currentUserService) : ICategoryService
{
    public Task<CategoryResponse> Get(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<CategoryResponse[]> GetAll(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<CreatedResponse> Create(CategoryRequest request, CancellationToken cancellationToken)
    {
        var userId = await currentUserService.GetUserIdAsync(cancellationToken);
        
        if (userId == null)
        {
            throw new ArgumentException("User not found");
        }
        
        var category = CategoriesMappers.MapToDomain(request);
        category.Id = Guid.NewGuid().ToString();
        category.UserId = userId;
        
        await categoryRepository.Create(category, cancellationToken);

        return new CreatedResponse(category.Id);
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