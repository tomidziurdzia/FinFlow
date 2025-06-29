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
            Type = request.Type,
            Color = request.Color,
            Icon = request.Icon
        };
    }
    
    internal static Category MapFromUpdateRequest(CategoryRequest request, Category category)
    {
        category.Name = request.Name;
        category.Type = request.Type;
        category.Color = request.Color;
        category.Icon = request.Icon;
        
        return category;
    }

    internal static CategoryResponse MapFromEntity(Category category)
    {
        return new CategoryResponse(
            Id: category.Id,
            Name: category.Name,
            Type: category.Type,
            Color: category.Color,
            Icon: category.Icon
        );
    }
}