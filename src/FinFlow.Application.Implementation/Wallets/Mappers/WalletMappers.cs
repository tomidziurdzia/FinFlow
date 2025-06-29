using FinFlow.Application.Contracts.Wallets.Request;
using FinFlow.Application.Contracts.Wallets.Response;
using FinFlow.Domain;

namespace FinFlow.Application.Implementation.Wallets.Mappers;

public static class WalletMappers
{
    internal static Wallet MapFromCreateRequest(CreateWalletRequest request, string userId)
    {
        return new Wallet
        {
            Name = request.Name,
            Type = request.Type,
            Balance = request.Balance,
            Currency = request.Currency,
            Description = request.Description,
            UserId = userId
        };
    }

    internal static Wallet MapFromUpdateRequest(UpdateWalletRequest request, Wallet wallet)
    {
        wallet.Name = request.Name;
        wallet.Type = request.Type;
        wallet.Balance = request.Balance;
        wallet.Currency = request.Currency;
        wallet.Description = request.Description;

        return wallet;
    }

    internal static WalletResponse MapFromEntity(Wallet wallet)
    {
        return new WalletResponse
        {
            Id = wallet.Id,
            Name = wallet.Name,
            Type = wallet.Type,
            Balance = wallet.Balance,
            Currency = wallet.Currency,
            Description = wallet.Description
        };
    }
}