using HubtelWallets.API.Data;
using HubtelWallets.API.DTOs;
using HubtelWallets.API.Enums;
using HubtelWallets.API.Models;
using HubtelWallets.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace HubtelWallets.Tests;

public class WalletServiceTests
{
    private readonly WalletContext _context;
    private readonly Mock<ILogger<WalletService>> _mockLogger = new();
    private readonly WalletService _walletService;
    private readonly WalletValidationService _walletValidationService;

    public WalletServiceTests()
    {
        var options = new DbContextOptionsBuilder<WalletContext>()
            .UseInMemoryDatabase(databaseName: "WalletTestDb")
            .Options;
        _context = new WalletContext(options);
        _mockLogger = new Mock<ILogger<WalletService>>();
        _walletValidationService = new WalletValidationService(_context);
        _walletService = new WalletService(_context, _mockLogger.Object, _walletValidationService);
    }
    [Fact]
    public async Task AddWalletAsync_ValidWallet_ReturnsWalletResponseDto()
    {
        // Arrange

        var walletDto = new WalletDto("Prince", "4141111111111111", "visa", "card", "0240123456");

        // Act
        var result = await _walletService.AddWalletAsync(walletDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(walletDto.Name, result.Name);
        Assert.Equal(walletDto.Owner, result.Owner);
        Assert.IsType<WalletResponseDto>(result);
        _mockLogger.Verify(
                 x => x.Log(
                     LogLevel.Information,
                     It.IsAny<EventId>(),
                     It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Wallet added successfully")),
                     It.IsAny<Exception>(),
                     It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                 Times.Once);
    }

    [Fact]
    public async Task AddWalletAsync_NullWalletDto_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _walletService.AddWalletAsync(null));
    }

    [Fact]
    public async Task CanAddMoreWalletsAsync_LessThanFiveWallets_ReturnsTrue()
    {
        // Arrange
        var phoneNumber = "0240123456";
        for (int i = 0; i < 4; i++)
        {
            _context.Wallets.Add(new Wallet
            {
                Id = Guid.NewGuid(),
                Name = $"Wallet {i + 1}",
                AccountNumber = $"411111111111111{i}",
                AccountScheme = WalletAccountScheme.visa,
                Type = WalletType.card,
                Owner = phoneNumber,
                CreatedAt = DateTime.UtcNow
            });
        }
        await _context.SaveChangesAsync();

        // Act
        var result = await _walletValidationService.CanAddMoreWalletsAsync(phoneNumber);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CanAddMoreWalletsAsync_FiveOrMoreWallets_ReturnsFalse()
    {
        // Arrange
        var phoneNumber = "0240123456";
        for (int i = 0; i < 5; i++)
        {
            _context.Wallets.Add(new Wallet
            {
                Id = Guid.NewGuid(),
                Name = $"Wallet {i + 1}",
                AccountNumber = $"411111111111111{i}",
                AccountScheme = WalletAccountScheme.visa,
                Type = WalletType.card,
                Owner = phoneNumber,
                CreatedAt = DateTime.UtcNow
            });
        }
        await _context.SaveChangesAsync();

        // Act
        var result = await _walletValidationService.CanAddMoreWalletsAsync(phoneNumber);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetWalletAsync_ValidId_ReturnsWalletResponseDto()
    {
        // Arrange
        var walletId = Guid.NewGuid();
        var wallet = new Wallet
        {
            Id = walletId,
            Name = "Ben",
            AccountNumber = "0200000000",
            AccountScheme = WalletAccountScheme.vodafone,
            Type = WalletType.momo,
            Owner = "0200000000",
            CreatedAt = DateTime.UtcNow

        };

        // Act
        await _context.Wallets.AddAsync(wallet);
        await _context.SaveChangesAsync();
        var result = await _walletService.GetWalletAsync(walletId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(wallet.Name, result.Name);
        Assert.Equal(wallet.Owner, result.Owner);
    }

    [Fact]
    public async Task GetWalletsAsync_ReturnsPaginatedWallets()
    {
        // Arrange
        for (int i = 0; i < 15; i++)
        {
            _context.Wallets.Add(new Wallet
            {
                Id = Guid.NewGuid(),
                Name = $"Wallet {i + 1}",
                AccountNumber = $"411111111111111{i}",
                AccountScheme = WalletAccountScheme.visa,
                Type = WalletType.card,
                Owner = "0240123456",
                CreatedAt = DateTime.UtcNow
            });
        }
        await _context.SaveChangesAsync();

        // Act
        var result = await _walletService.GetWalletsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Data.Count()); 
        Assert.Equal(15, result.Meta.TotalCount);
    }

    [Fact]
    public async Task GetWalletsAsync_ReturnsEmptyWhenNoWallets()
    {
        // Act
        var result = await _walletService.GetWalletsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Data);
        Assert.Equal(0, result.Meta.TotalCount);
    }

    [Fact]
    public async Task GetWalletAsync_InvalidId_ThrowsException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _walletService.GetWalletAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task RemoveWalletAsync_ValidId_ReturnsTrue()
    {
        // Arrange
        var walletId = Guid.NewGuid();

        // Act
        var wallet = new Wallet
        {
            Id = walletId,
            Name = "Ben",
            AccountNumber = "0200000000",
            AccountScheme = WalletAccountScheme.vodafone,
            Type = WalletType.momo,
            Owner = "0200000000",
            CreatedAt = DateTime.UtcNow

        };
        await _context.Wallets.AddAsync(wallet);
        await _context.SaveChangesAsync();
        var result = await _walletService.RemoveWalletAsync(walletId);

        // Assert
        Assert.True(result);

    }

}