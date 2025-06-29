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
        
        group.MapGet("/{id}", async (
                [FromServices] ICategoryService categoryService,
                [FromRoute] string id,
                CancellationToken cancellationToken) =>
            {
                var result = await categoryService.Get(id, cancellationToken);
                return Results.Ok(result);
            })
            .WithName("GetCategory")
            .Produces<CategoryResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();
        
        group.MapGet("/", async (
                [FromServices] ICategoryService categoryService,
                CancellationToken cancellationToken) =>
            {
                var result = await categoryService.GetAll(cancellationToken);
                return Results.Ok(result);
            })
            .WithName("GetAllCategories")
            .Produces<CategoryResponse[]>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();

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
        
        group.MapPut("/{id}", async (
                [FromServices] ICategoryService categoryService,
                [FromRoute] string id,
                [FromBody] CategoryRequest request,
                CancellationToken cancellationToken) =>
            {
                await categoryService.Update(id, request, cancellationToken);
                return Results.Ok();
            })
            .WithName("UpdateCategory")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();
        
        group.MapDelete("/{id}", async (
                [FromServices] ICategoryService categoryService,
                [FromRoute] string id,
                CancellationToken cancellationToken) =>
            {
                await categoryService.Delete(id, cancellationToken);
                return Results.NoContent();
            })
            .WithName("DeleteCategory")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();
        
        return endpointRouteBuilder;
    }
}