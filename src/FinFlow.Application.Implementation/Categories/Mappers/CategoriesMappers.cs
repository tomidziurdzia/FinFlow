using FinFlow.Application.Contracts.Categories.Response;
using FinFlow.Domain;

namespace FinFlow.Application.Implementation.Categories.Mappers;

public static class CategoriesMappers
{
    internal static Category MapFromCreateRequest(this Contracts.Categories.Request.CategoryRequest request)
    {
        return new Category
        {
            Name = request.Name,
            Type = request.Type
        };
    }

    internal static CategoryResponse MapFromEntity(Category category)
    {
        return new CategoryResponse(
            Name: category.Name,
            Type: category.Type
        );
    }
}