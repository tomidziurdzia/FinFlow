namespace FinFlow.Application.Contracts.Users;

public interface ICurrentUserService
{
    string? AuthId { get; }
    Task<string?> GetUserIdAsync(CancellationToken cancellationToken);
}