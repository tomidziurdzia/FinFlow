using System.Security.Claims;
using FinFlow.Application.Contracts.Users;
using FinFlow.Domain.Repositories;
using Microsoft.AspNetCore.Http;

namespace FinFlow.Infrastructure.Services.User;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository) : ICurrentUserService
{
    public string AuthId =>
        httpContextAccessor.HttpContext?
            .User?
            .Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value!;
    
    public async Task<string> GetUserIdAsync(CancellationToken cancellationToken) => await userRepository.GetUserIdByAuthId(AuthId, cancellationToken);
}