using System.Data.Entity;
using BntWeb.SinglePage.Models;
using BntWeb.Data;


namespace BntWeb.SinglePage
{
    public class SinglePageDbContext : BaseDbContext
    {
        public DbSet<Models.SinglePage> SinglePage { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}