using FinFlow.Application.Contracts.Users;
using FinFlow.Application.Contracts.Users.Request;
using FinFlow.Application.Contracts.Users.Response;
using FinFlow.WebApi.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinFlow.WebApi.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder RegisterUserEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var group = endpointRouteBuilder.MapGroup("/users");

        group.MapPost("/sync", async (
                HttpContext httpContext,
                [FromServices] IUserService userService,
                CancellationToken cancellationToken) =>
            {
                var user = httpContext.User;

                var authId = user.GetUserId() ?? "";
                var firstName = user.GetFirstName() ?? "";
                var lastName = user.GetLastName() ?? "";

                var request = new UserRequest(authId, firstName, lastName);

                var result = await userService.GetUserByAuthId(request, cancellationToken);

                return Results.Ok(result);
            })
            .WithName("SyncUser")
            .Produces<UserResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();

        group.MapPost("/", async (
                [FromBody] CreateUserRequest request,
                [FromServices] IUserService userService,
                CancellationToken cancellationToken) =>
            {
                var result = await userService.CreateUserFormAuth(request, cancellationToken);

                return Results.Ok(result);
            })
            .WithName("CreateUserFromAuth")
            .Produces<UserResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .AllowAnonymous();

        group.MapPost("/login", async (
                [FromBody] LoginRequest request,
                [FromServices] IUserService userService,
                CancellationToken cancellationToken) =>
            {
                try
                {
                    var result = await userService.Login(request, cancellationToken);
                    return Results.Ok(result);
                }
                catch (InvalidOperationException ex)
                {
                    return Results.BadRequest(new { message = ex.Message });
                }
            })
            .WithName("Login")
            .Produces<LoginResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .AllowAnonymous();

        group.MapGet("/me", async (
                HttpContext httpContext,
                [FromServices] IUserService userService,
                CancellationToken cancellationToken) =>
            {
                var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Results.Unauthorized();
                }

                var user = await userService.GetUser(userId, cancellationToken);
                if (user == null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(user);
            })
            .WithName("GetCurrentUser")
            .Produces<UserResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();

        return endpointRouteBuilder;
    }
}