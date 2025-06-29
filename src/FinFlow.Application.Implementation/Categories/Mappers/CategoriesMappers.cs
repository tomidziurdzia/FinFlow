using FinFlow.Application.Contracts.Categories.Request;
using FinFlow.Application.Contracts.Categories.Response;
using FinFlow.Domain;

namespace FinFlow.Application.Implementation.Categories.Mappers;

public static class CategoriesMappers
{
    internal static Category MapFromCreateRequest(CategoryRequest request)
    {
        return new Category
        {
            Name = request.Name,
            Type = request.Type
        };
    }
    
    internal static Category MapFromUpdateRequest(CategoryRequest request, Category category)
    {
        category.Name = request.Name;
        category.Type = request.Type;
        
        return category;
    }

    internal static CategoryResponse MapFromEntity(Category category)
    {
        return new CategoryResponse(
            Id: category.Id,
            Name: category.Name,
            Type: category.Type
        );
    }
}