using System.Data.Entity;
using BntWeb.Data;

namespace BntWeb.Discovery
{
    public class DiscoveryDbContext : BaseDbContext
    {
        public DbSet<Models.Discovery> Discoveries { get; set; }

        public DbSet<Models.DiscoveryGoodsRelation> DiscoveryGoodsRelations { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}