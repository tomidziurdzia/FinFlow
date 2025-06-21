namespace FinFlow.Application.Contracts.Users.Response;

public record UserResponse(Guid Id, string AuthId, string FirstName, string LastName);