using HubtelWallets.API.Data;
using HubtelWallets.API.Enums;
using HubtelWallets.API.Models;
using HubtelWallets.API.Services;
using Microsoft.EntityFrameworkCore;

namespace HubtelWallets.Tests;
public class WalletValidationServiceTests
{
    private readonly WalletContext _context;
    private readonly WalletValidationService _walletValidationService;

    public WalletValidationServiceTests()
    {
        var options = new DbContextOptionsBuilder<WalletContext>()
            .UseInMemoryDatabase(databaseName: "WalletTestDb")
            .Options;
        _context = new WalletContext(options);
        _walletValidationService = new WalletValidationService(_context);
    }

    [Fact]
    public void ValidateWallet_InvalidWalletType_ThrowsArgumentException()
    {
        // Arrange
        var wallet = new Wallet
        {
            Id = Guid.NewGuid(),
            Name = "Test Wallet",
            AccountNumber = "4111111111111111",
            AccountScheme = WalletAccountScheme.visa,
            Type = (WalletType)999,
            Owner = "Test Owner",
            CreatedAt = DateTime.UtcNow
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _walletValidationService.ValidateWallet(wallet));
        Assert.Equal("Invalid wallet type. Only 'momo' or 'card' are accepted.", exception.Message);
    }

    [Fact]
    public void ValidateWallet_InvalidAccountScheme_ThrowsArgumentException()
    {
        // Arrange
        var wallet = new Wallet
        {
            Id = Guid.NewGuid(),
            Name = "Test Wallet",
            AccountNumber = "4111111111111111",
            AccountScheme = (WalletAccountScheme)999,
            Type = WalletType.card,
            Owner = "Test Owner",
            CreatedAt = DateTime.UtcNow
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _walletValidationService.ValidateWallet(wallet));
        Assert.Equal("Invalid account scheme provided.", exception.Message);
    }

    [Theory]
    [InlineData("4111111111111111", WalletAccountScheme.visa, true)]
    [InlineData("5500000000000004", WalletAccountScheme.mastercard, true)]
    [InlineData("4111111111111", WalletAccountScheme.visa, false)] // Invalid length for Visa card
    [InlineData("5500000000000004", WalletAccountScheme.visa, false)] // Valid Mastercard but wrong scheme
    [InlineData("1234567890123456", WalletAccountScheme.visa, false)] // Invalid Visa card number
    public void IsValidCardNumber_ValidatesCorrectly(string cardNumber, WalletAccountScheme accountScheme, bool expected)
    {
        // Act
        var result = _walletValidationService.IsValidCardNumber(cardNumber, accountScheme);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task IsAccountNumberUniqueAsync_UniqueAccountNumber_ReturnsTrue()
    {
        // Arrange
        var accountNumber = "4111111111111111";

        // Act
        var result = await _walletValidationService.IsAccountNumberUniqueAsync(accountNumber);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsAccountNumberUniqueAsync_NonUniqueAccountNumber_ReturnsFalse()
    {
        // Arrange
        var accountNumber = "4111111111111111";
        _context.Wallets.Add(new Wallet
        {
            Id = Guid.NewGuid(),
            Name = "Test Wallet",
            AccountNumber = accountNumber,
            AccountScheme = WalletAccountScheme.visa,
            Type = WalletType.card,
            Owner = "0240123456",
            CreatedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        // Act
        var result = await _walletValidationService.IsAccountNumberUniqueAsync(accountNumber);

        // Assert
        Assert.False(result);
    }
}
