using HubtelWallets.API.Data;
using HubtelWallets.API.DTOs;
using HubtelWallets.API.Enums;
using HubtelWallets.API.Helpers;
using HubtelWallets.API.Models;

namespace HubtelWallets.API.Services;

public class WalletService(WalletContext context, ILogger<WalletService> logger) : IWalletService
{
    private readonly WalletContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<WalletService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<WalletResponseDto> AddWalletAsync(WalletDto walletDto)
    {
        if (walletDto == null)
        {
            throw new ArgumentNullException(nameof(walletDto));
        }

        var wallet = new Wallet
        {
            Id = Guid.NewGuid(),
            Name = walletDto.Name,
            AccountNumber = walletDto.AccountNumber,
            AccountScheme = walletDto.AccountScheme.GetEnumValue<WalletAccountScheme>(),
            Type = walletDto.Type.GetEnumValue<WalletType>(),
            Owner = walletDto.Owner,
            CreatedAt = DateTime.UtcNow
        };

        ValidateWallet(wallet);

        await _context.Wallets.AddAsync(wallet);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Wallet added successfully: {WalletId}", wallet.Id);

        return WalletResponseDto.FromWallet(wallet);

    }


    public async Task<WalletResponseDto> GetWalletAsync(Guid id)
    {
        var wallet = await _context.Wallets.FindAsync(id) ??
            throw new Exception($"Wallet not found");
        _logger.LogWarning("Wallet not found: {WalletId}", id);

        return WalletResponseDto.FromWallet(wallet);
    }

    public async Task<PaginationInfo<WalletResponseDto>> GetWalletsAsync()
    {
        var query = _context.Wallets.Select(w => new WalletResponseDto(
            w.Id,
            w.Name,
            w.AccountNumber,
            w.AccountScheme.ToString(),
            w.Type.ToString(),
            w.Owner,
            w.CreatedAt
        ));
        var pagedResponse = await PagedList<WalletResponseDto>.ToPageableAsync(query, 1, 10);


        return new PaginationInfo<WalletResponseDto>
        {
            Data = pagedResponse,
            Meta = pagedResponse.Meta
        };

    }

    public async Task<bool> RemoveWalletAsync(Guid id)
    {
        var wallet = await _context.Wallets.FindAsync(id);
        if (wallet == null)
        {
            _logger.LogWarning("Wallet not found: {WalletId}", id);
            return false;
        }

        _context.Wallets.Remove(wallet);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Wallet removed successfully: {WalletId}", id);
        return true;
    }

    internal void ValidateWallet(Wallet wallet)
    {
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

        if (!Enum.IsDefined(typeof(WalletAccountScheme), wallet.AccountScheme))
        {
            throw new ArgumentException("Invalid account scheme provided.");
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
