using HubtelWallets.API.Models;

namespace HubtelWallets.API.DTOs;

public record WalletResponseDto(Guid Id, string Name, string AccountNumber, string AccountScheme, string Type, string Owner, DateTime CreatedAt)
{
    public static WalletResponseDto FromWallet(Wallet wallet)
    {
        if (wallet is null) throw new ArgumentNullException(nameof(wallet));

        return new WalletResponseDto(wallet.Id,
            wallet.Name,
            wallet.AccountNumber,
            wallet.AccountScheme.ToString(),
            wallet.Type.ToString(),
            wallet.Owner,
            wallet.CreatedAt);
    }
}



