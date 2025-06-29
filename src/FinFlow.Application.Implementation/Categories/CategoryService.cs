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
    public async Task<CategoryResponse> Get(string id, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.Get(id, cancellationToken);
        if (category == null)
        {
            throw new KeyNotFoundException($"Category with id '{id}' not found.");
        }
        
        return CategoriesMappers.MapFromEntity(category);
    }

    public async Task<CategoryResponse[]> GetAll(CancellationToken cancellationToken)
    {
        var categories = await categoryRepository.GetAll(cancellationToken);
        return categories.Select(CategoriesMappers.MapFromEntity).ToArray();
    }

    public async Task<CreatedResponse> Create(CategoryRequest request, CancellationToken cancellationToken)
    {
        var userId = await currentUserService.GetUserIdAsync(cancellationToken);
        var category = CategoriesMappers.MapFromCreateRequest(request);
        category.Id = Guid.NewGuid().ToString();
        category.UserId = userId;
        
        await categoryRepository.Create(category, cancellationToken);

        return new CreatedResponse(category.Id);
    }

    public async Task Update(string id, CategoryRequest request, CancellationToken cancellationToken)
    {
        var existingCategory = await categoryRepository.Get(id, cancellationToken);
        if (existingCategory == null)
        {
            throw new KeyNotFoundException($"Category with id '{id}' not found.");
        }
        
        var category = CategoriesMappers.MapFromUpdateRequest(request, existingCategory);

        await categoryRepository.Update(category, cancellationToken);
    }

    public async Task Delete(string id, CancellationToken cancellationToken)
    {
        var existingCategory = await categoryRepository.Get(id, cancellationToken);
        if (existingCategory == null)
        {
            throw new KeyNotFoundException($"Category with id '{id}' not found.");
        }

        await categoryRepository.Delete(existingCategory.Id, cancellationToken);
    }
}