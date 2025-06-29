using FinFlow.Application.Contracts.Shared;
using FinFlow.Application.Contracts.Transactions;
using FinFlow.Application.Contracts.Transactions.Request;
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

        return endpointRouteBuilder;
    }
}