using System.Data.Entity;
using BntWeb.Coupon;
using BntWeb.Data;


namespace BntWeb.Coupon
{
    public class CouponDbContext: BaseDbContext
    {
        public DbSet<Models.Coupon> Coupon { get; set; }

        public DbSet<Models.CouponRelation> CouponRelation { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}