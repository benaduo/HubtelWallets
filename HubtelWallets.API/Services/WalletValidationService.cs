using HubtelWallets.API.Data;
using HubtelWallets.API.Enums;
using HubtelWallets.API.Models;
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

    public void ValidateWallet(Wallet wallet)
    {
        if (!Enum.IsDefined(typeof(WalletAccountScheme), wallet.AccountScheme))
        {
            throw new ArgumentException("Invalid account scheme provided.");
        }
        if (wallet.Type == WalletType.card)
        {
            if (!IsValidCardNumber(wallet.AccountNumber, wallet.AccountScheme))
            {
                throw new ArgumentException("Invalid card number provided for the specified Account Scheme.");
            }
            wallet.AccountNumber = wallet.AccountNumber.Substring(0, 6);
        }

        if (wallet.Type != WalletType.momo && wallet.Type != WalletType.card)
        {
            throw new ArgumentException("Invalid wallet type. Only 'momo' or 'card' are accepted.");
        }

    }

    internal bool IsValidCardNumber(string cardNumber, WalletAccountScheme accountScheme)
    {
        cardNumber = new string(cardNumber.Where(char.IsDigit).ToArray());

        return accountScheme switch
        {
            WalletAccountScheme.visa => cardNumber.StartsWith("4") && cardNumber.Length == 16,
            WalletAccountScheme.mastercard => (cardNumber.StartsWith("51") || cardNumber.StartsWith("52") ||
                                               cardNumber.StartsWith("53") || cardNumber.StartsWith("54") ||
                                               cardNumber.StartsWith("55") ||
                                               (cardNumber.CompareTo("2221000000000000") >= 0 &&
                                                cardNumber.CompareTo("2720999999999999") <= 0)) &&
                                              cardNumber.Length == 16,
            _ => false,
        };
    }
}
