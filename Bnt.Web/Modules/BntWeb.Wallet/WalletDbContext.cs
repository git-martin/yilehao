
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using BntWeb.Data;

namespace BntWeb.Wallet
{
    
    public class WalletDbContext : BaseDbContext
    {
        public DbSet<Models.Wallet> Wallets { get; set; }


        public DbSet<Models.WalletBill> WalletBills { get; set; }

        public DbSet<Models.CrashApply> CrashApplys { get; set; }

    }
}
