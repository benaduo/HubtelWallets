using HubtelWallets.API.Data;
using Microsoft.EntityFrameworkCore;

namespace HubtelWallets.API.Services;

public class WalletValidationService(WalletContext context) : IWalletValidationService
{
    public async Task<bool> CanAddMoreWalletsAsync(string phoneNumber)
    {
        var walletCount = await context.Wallets.CountAsync(w => w.Owner == phoneNumber);
        return walletCount < 5;
    }

    public async Task<bool> IsAccountNumberUniqueAsync(string accountNumber)
    {
        return !await context.Wallets.AnyAsync(w => w.AccountNumber == accountNumber);
    }

}
