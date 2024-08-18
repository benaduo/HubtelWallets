using HubtelWallets.API.Models;

namespace HubtelWallets.API.Services;

public interface IWalletValidationService
{
    Task<bool> IsAccountNumberUniqueAsync(string accountNumber);
    Task<bool> CanAddMoreWalletsAsync(string phoneNumber);
    void ValidateWallet(Wallet wallet);
}
