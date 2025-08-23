namespace FinFlow.Application.Contracts.Users.Request;

public record CreateUserRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName);