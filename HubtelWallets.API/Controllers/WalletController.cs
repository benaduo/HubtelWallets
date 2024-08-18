using HubtelWallets.API.DTOs;
using HubtelWallets.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace HubtelWallets.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WalletController(IWalletService walletService, IWalletValidationService walletValidationService) : ControllerBase
{
    /// <summary>
    /// Adds a new wallet.
    /// </summary>
    /// <param name="walletDto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> AddWalletAsync([FromBody] WalletDto walletDto)
    {

        if (walletDto is null) return BadRequest(ModelState);

        if (!await walletValidationService.IsAccountNumberUniqueAsync(walletDto.AccountNumber))
        {
            return BadRequest("The account number is already in use by another owner.");
        }

        if (!await walletValidationService.CanAddMoreWalletsAsync(walletDto.Owner))
        {
            return BadRequest("The owner has reached the maximum number of wallets (5).");
        }
        try
        {

            var result = await walletService.AddWalletAsync(walletDto);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {

            return BadRequest(ex.Message);
        }
    }
    /// <summary>
    /// Gets all wallets.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetWalletsAsync()
    {
        var result = await walletService.GetWalletsAsync();
        return Ok(result);
    }

    /// <summary>
    /// Gets a single wallet by id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetWalletById(Guid id)
    {
        if (id == Guid.Empty) return BadRequest("Invalid wallet id provided.");

        var wallet = await walletService.GetWalletAsync(id);

        return wallet switch
        {
            null => NotFound(),
            _ => Ok(wallet)
        };
    }

    /// <summary>
    /// Removes a wallet by id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteWallet(Guid id)
    {
        if (id == Guid.Empty) return BadRequest("Invalid wallet id provided.");

        var wallet = await walletService.GetWalletAsync(id);

        if (wallet is null) return NotFound();

        await walletService.RemoveWalletAsync(id);

        return NoContent();
    }
}

