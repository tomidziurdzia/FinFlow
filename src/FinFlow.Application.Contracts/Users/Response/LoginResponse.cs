namespace FinFlow.Application.Contracts.Users.Response;

public record LoginResponse(
    string Token,
    UserResponse User);
