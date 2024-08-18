using HubtelWallets.API.Controllers;
using HubtelWallets.API.DTOs;
using HubtelWallets.API.Helpers;
using HubtelWallets.API.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HubtelWallets.Tests;
public class WalletControllerTests
{
    private readonly Mock<IWalletService> _mockWalletService = new();
    private readonly Mock<IWalletValidationService> _mockWalletValidationService = new();
    private readonly WalletController _walletController;
    public WalletControllerTests()
    {
        _mockWalletService = new Mock<IWalletService>();
        _walletController = new WalletController(_mockWalletService.Object, _mockWalletValidationService.Object);
    }

    [Fact]
    public async Task GetWallets_ReturnsOkResult_WithWallets()
    {
        // Arrange

        var pagedResponse = new PaginationInfo<WalletResponseDto>
        {
            Data = new List<WalletResponseDto>
            {
                new WalletResponseDto(Guid.NewGuid(), "Ben", "0261234567", "airteltigo", "momo", "0261234567", DateTime.UtcNow),
                new WalletResponseDto(Guid.NewGuid(), "Ben", "0261234567", "airteltigo", "momo", "0261234567", DateTime.UtcNow)
            },
            Meta = new Metadata
            {
                TotalCount = 2,
                PageSize = 10 ,
                CurrentPage = 1,
                TotalPages = 1
            }
        };

        _mockWalletService.Setup(s => s.GetWalletsAsync()).ReturnsAsync(pagedResponse);

        // Act
        var result = await _walletController.GetWalletsAsync();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<PaginationInfo<WalletResponseDto>>(okResult.Value);
        Assert.Equal(2, returnValue.Data.Count());
    }

    [Fact]
    public async Task AddWallet_ReturnsBadRequest_WhenWalletDtoIsNull()
    {

        // Act
        var result = await _walletController.AddWalletAsync(null);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetWalletById_ReturnsBadRequest_WhenIdIsEmpty()
    {
        // Act
        var result = await _walletController.GetWalletById(Guid.Empty);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid wallet id provided.", badRequestResult.Value);
    }

    [Fact]
    public async Task GetWalletById_ReturnsNotFound_WhenWalletDoesNotExist()
    {
        // Arrange
        var walletId = Guid.NewGuid();

        _mockWalletService.Setup(service => service.GetWalletAsync(walletId))
            .ReturnsAsync((WalletResponseDto)null);

        // Act
        var result = await _walletController.GetWalletById(walletId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteWallet_ReturnsNotFound_WhenWalletDoesNotExist()
    {
        // Arrange
        var walletId = Guid.NewGuid();

        _mockWalletService.Setup(service => service.GetWalletAsync(walletId))
            .ReturnsAsync((WalletResponseDto)null);

        // Act
        var result = await _walletController.DeleteWallet(walletId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteWallet_ReturnsBadRequest_WhenIdIsEmpty()
    {
        // Act
        var result = await _walletController.DeleteWallet(Guid.Empty);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid wallet id provided.", badRequestResult.Value);
    }


    [Fact]
    public async Task DeleteWallet_ReturnsNoContent_WhenWalletIsDeleted()
    {
        // Arrange
        var walletId = Guid.NewGuid();
        var walletResponseDto = new WalletResponseDto(walletId, "Wallet 1", "411111", "visa", "card", "0240123456", DateTime.UtcNow);

        _mockWalletService.Setup(service => service.GetWalletAsync(walletId))
            .ReturnsAsync(walletResponseDto);

        // Act
        var result = await _walletController.DeleteWallet(walletId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }
}
