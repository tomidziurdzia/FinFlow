using FinFlow.Application.Contracts.Wallets;
using FinFlow.Application.Contracts.Wallets.Request;
using FinFlow.Application.Contracts.Wallets.Response;
using Microsoft.AspNetCore.Mvc;

namespace FinFlow.WebApi.Endpoints;

public static class WalletEndpoints
{
    public static IEndpointRouteBuilder RegisterWalletEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var group = endpointRouteBuilder.MapGroup("/wallets");

        group.MapGet("/{id}", async (
                [FromServices] IWalletService walletService,
                [FromRoute] string id,
                CancellationToken cancellationToken) =>
            {
                var result = await walletService.Get(id, cancellationToken);
                return Results.Ok(result);
            })
            .WithName("GetWallet")
            .Produces<WalletResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();

        group.MapGet("/", async (
                [FromServices] IWalletService walletService,
                CancellationToken cancellationToken) =>
            {
                var result = await walletService.GetAll(cancellationToken);
                return Results.Ok(result);
            })
            .WithName("GetAllWallets")
            .Produces<WalletResponse[]>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();

        group.MapPost("/", async (
                [FromServices] IWalletService walletService,
                [FromBody] CreateWalletRequest request,
                CancellationToken cancellationToken) =>
            {
                var result = await walletService.Create(request, cancellationToken);
                return Results.Ok(result);
            })
            .WithName("CreateWallet")
            .Produces<WalletResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization();

        group.MapPut("/{id}", async (
                [FromServices] IWalletService walletService,
                [FromRoute] string id,
                [FromBody] UpdateWalletRequest request,
                CancellationToken cancellationToken) =>
            {
                await walletService.Update(id, request, cancellationToken);
                return Results.Ok();
            })
            .WithName("UpdateWallet")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();

        group.MapDelete("/{id}", async (
                [FromServices] IWalletService walletService,
                [FromRoute] string id,
                CancellationToken cancellationToken) =>
            {
                await walletService.Delete(id, cancellationToken);
                return Results.NoContent();
            })
            .WithName("DeleteWallet")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();

        return endpointRouteBuilder;
    }
}