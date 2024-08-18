using HubtelWallets.API.Enums;

namespace HubtelWallets.API.Models;

public class Wallet
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public WalletType Type { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public WalletAccountScheme AccountScheme { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Owner { get; set; } = string.Empty;
}
