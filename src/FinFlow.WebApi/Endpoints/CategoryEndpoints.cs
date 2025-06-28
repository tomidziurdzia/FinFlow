using FinFlow.Application.Contracts.Categories;
using FinFlow.Application.Contracts.Categories.Request;
using FinFlow.Application.Contracts.Categories.Response;
using FinFlow.WebApi.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace FinFlow.WebApi.Endpoints;

public static class CategoryEndpoints
{
    public static IEndpointRouteBuilder RegisterCategoryEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var group = endpointRouteBuilder.MapGroup("/categories");

        group.MapPost("/", async (
                [FromServices] ICategoryService categoryService,
                [FromBody] CategoryRequest request,
                CancellationToken cancellationToken) =>
            {
                var result = await categoryService.Create(request, cancellationToken);
                return Results.Ok(result);
            })
            .WithName("CreateCategory")
            .Produces<CategoryResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization();
        return endpointRouteBuilder;
    }
}