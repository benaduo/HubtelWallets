using HubtelWallets.API.DTOs;
using HubtelWallets.API.Helpers;

namespace HubtelWallets.API.Services;

public interface IWalletService
{
    Task<WalletResponseDto> AddWalletAsync(WalletDto wallet);
    Task<WalletResponseDto> GetWalletAsync(Guid id);
    Task<bool> RemoveWalletAsync(Guid id);
    Task<PaginationInfo<WalletResponseDto>> GetWalletsAsync();
}
