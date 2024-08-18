using HubtelWallets.API.Models;
using Microsoft.EntityFrameworkCore;

namespace HubtelWallets.API.Data;

public class WalletContext : DbContext
{

    public WalletContext(DbContextOptions<WalletContext> options) : base(options)
    {

    }
    public DbSet<Wallet> Wallets { get; set; }
}
