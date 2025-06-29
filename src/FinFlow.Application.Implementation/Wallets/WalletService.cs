using FinFlow.Application.Contracts.Shared;
using FinFlow.Application.Contracts.Users;
using FinFlow.Application.Contracts.Wallets;
using FinFlow.Application.Contracts.Wallets.Request;
using FinFlow.Application.Contracts.Wallets.Response;
using FinFlow.Application.Implementation.Wallets.Mappers;
using FinFlow.Domain.Repositories;

namespace FinFlow.Application.Implementation.Wallets;

public class WalletService(IWalletRepository walletRepository, ICurrentUserService currentUserService) : IWalletService
{
    public async Task<WalletResponse> Get(string id, CancellationToken cancellationToken)
    {
        var wallet = await walletRepository.Get(id, cancellationToken);
        if (wallet == null)
        {
            throw new KeyNotFoundException($"Wallet with id '{id}' not found.");
        }

        var currentUserId = await currentUserService.GetUserIdAsync(cancellationToken);
        if (wallet.UserId != currentUserId)
            throw new UnauthorizedAccessException("You can only access your own wallets.");

        return WalletMappers.MapFromEntity(wallet);
    }

    public async Task<WalletResponse[]> GetAll(CancellationToken cancellationToken)
    {
        var currentUserId = await currentUserService.GetUserIdAsync(cancellationToken);
        var wallets = await walletRepository.GetAll(cancellationToken);

        var userWallets = wallets.Where(w => w.UserId == currentUserId);

        return userWallets.Select(WalletMappers.MapFromEntity).ToArray();
    }

    public async Task<CreatedResponse> Create(CreateWalletRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = await currentUserService.GetUserIdAsync(cancellationToken);

        var wallet = WalletMappers.MapFromCreateRequest(request, currentUserId);
        wallet.Id = Guid.NewGuid().ToString();

        await walletRepository.Create(wallet, cancellationToken);
        return new CreatedResponse(wallet.Id);
    }

    public async Task Update(string id, UpdateWalletRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = await currentUserService.GetUserIdAsync(cancellationToken);

        var wallet = await walletRepository.Get(id, cancellationToken);
        if (wallet == null)
            throw new KeyNotFoundException($"Wallet with id '{id}' not found.");

        if (wallet.UserId != currentUserId)
            throw new UnauthorizedAccessException("You can only update your own wallets.");

        WalletMappers.MapFromUpdateRequest(request, wallet);
        await walletRepository.Update(wallet, cancellationToken);
    }

    public async Task Delete(string id, CancellationToken cancellationToken)
    {
        var currentUserId = await currentUserService.GetUserIdAsync(cancellationToken);

        var wallet = await walletRepository.Get(id, cancellationToken);
        if (wallet == null)
            throw new KeyNotFoundException($"Wallet with id '{id}' not found.");

        if (wallet.UserId != currentUserId)
            throw new UnauthorizedAccessException("You can only delete your own wallets.");

        if (wallet.Transactions != null && wallet.Transactions.Any())
            throw new InvalidOperationException("Cannot delete a wallet with associated transactions.");

        await walletRepository.Delete(id, cancellationToken);
    }
}