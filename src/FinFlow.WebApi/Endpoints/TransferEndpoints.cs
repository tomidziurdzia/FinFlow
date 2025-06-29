using FinFlow.Application.Contracts.Shared;
using FinFlow.Application.Contracts.Transactions;
using FinFlow.Application.Contracts.Transactions.Request;
using FinFlow.Application.Contracts.Transactions.Response;
using Microsoft.AspNetCore.Mvc;

namespace FinFlow.WebApi.Endpoints;

public static class TransferEndpoints
{
    public static IEndpointRouteBuilder RegisterTransferEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var group = endpointRouteBuilder.MapGroup("/transfers");

        group.MapPost("/", async (
                [FromServices] ITransferService transferService,
                [FromBody] TransferRequest request,
                CancellationToken cancellationToken) =>
            {
                var result = await transferService.TransferBetweenWallets(request, cancellationToken);
                return Results.Ok(result);
            })
            .WithName("TransferBetweenWallets")
            .Produces<CreatedResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization();

        group.MapGet("/", async (
                [FromServices] ITransferService transferService,
                CancellationToken cancellationToken) =>
            {
                var result = await transferService.GetAll(cancellationToken);
                return Results.Ok(result);
            })
            .WithName("GetAllTransfers")
            .Produces<TransferResponse[]>(StatusCodes.Status200OK)
            .RequireAuthorization();

        group.MapGet("/{id}", async (
                [FromServices] ITransferService transferService,
                string id,
                CancellationToken cancellationToken) =>
            {
                var result = await transferService.Get(id, cancellationToken);
                return Results.Ok(result);
            })
            .WithName("GetTransferById")
            .Produces<TransferResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();

        group.MapPut("/{id}", async (
                [FromServices] ITransferService transferService,
                string id,
                [FromBody] UpdateTransferRequest request,
                CancellationToken cancellationToken) =>
            {
                await transferService.Update(id, request, cancellationToken);
                return Results.NoContent();
            })
            .WithName("UpdateTransfer")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();

        group.MapDelete("/{id}", async (
                [FromServices] ITransferService transferService,
                string id,
                CancellationToken cancellationToken) =>
            {
                await transferService.Delete(id, cancellationToken);
                return Results.NoContent();
            })
            .WithName("DeleteTransfer")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();

        return endpointRouteBuilder;
    }
}