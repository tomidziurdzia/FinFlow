using FinFlow.Application.Contracts.Users;
using FinFlow.Application.Contracts.Users.Request;
using FinFlow.Application.Contracts.Users.Response;
using FinFlow.WebApi.Extensions;
using Microsoft.AspNetCore.Mvc;

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

        return endpointRouteBuilder;
    }
}