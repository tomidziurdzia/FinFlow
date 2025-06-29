using FinFlow.Application.Contracts.Transactions;
using FinFlow.Application.Contracts.Transactions.Request;
using FinFlow.Application.Contracts.Transactions.Response;
using Microsoft.AspNetCore.Mvc;

namespace FinFlow.WebApi.Endpoints;

public static class TransactionEndpoints
{
    public static IEndpointRouteBuilder RegisterTransactionEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var group = endpointRouteBuilder.MapGroup("/transactions");

        group.MapGet("/{id}", async (
                [FromServices] ITransactionService transactionService,
                [FromRoute] string id,
                CancellationToken cancellationToken) =>
            {
                var result = await transactionService.Get(id, cancellationToken);
                return Results.Ok(result);
            })
            .WithName("GetTransaction")
            .Produces<TransactionResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();

        group.MapGet("/", async (
                [FromServices] ITransactionService transactionService,
                CancellationToken cancellationToken) =>
            {
                var result = await transactionService.GetAll(cancellationToken);
                return Results.Ok(result);
            })
            .WithName("GetAllTransactions")
            .Produces<TransactionResponse[]>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();

        group.MapPost("/", async (
                [FromServices] ITransactionService transactionService,
                [FromBody] CreateTransactionRequest request,
                CancellationToken cancellationToken) =>
            {
                var result = await transactionService.Create(request, cancellationToken);
                return Results.Ok(result);
            })
            .WithName("CreateTransaction")
            .Produces<TransactionResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization();

        group.MapPut("/{id}", async (
                [FromServices] ITransactionService transactionService,
                [FromRoute] string id,
                [FromBody] UpdateTransactionRequest request,
                CancellationToken cancellationToken) =>
            {
                await transactionService.Update(id, request, cancellationToken);
                return Results.Ok();
            })
            .WithName("UpdateTransaction")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();

        group.MapDelete("/{id}", async (
                [FromServices] ITransactionService transactionService,
                [FromRoute] string id,
                CancellationToken cancellationToken) =>
            {
                await transactionService.Delete(id, cancellationToken);
                return Results.NoContent();
            })
            .WithName("DeleteTransaction")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();

        return endpointRouteBuilder;
    }
}