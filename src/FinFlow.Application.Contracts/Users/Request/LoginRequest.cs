namespace FinFlow.Application.Contracts.Users.Request;

public record LoginRequest(
    string Email,
    string Password);
