
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using BntWeb.Data;
using BntWeb.OrderProcess.Models;

namespace BntWeb.OrderProcess
{
    
    public class OrderProcessDbContext : BaseDbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderGoods> OrderGoods { get; set; }
        public DbSet<OrderAction> OrderActions { get; set; }
        public DbSet<OrderRefund> OrderRefunds { get; set; }
        public DbSet<Evaluate.Models.Evaluate> Evaluates { get; set; }

        public DbSet<OrderDeliveryReminder> OrderDeliveryReminders { get; set; }
        public DbSet<ViewOrderRefund> ViewOrderRefunds { get; set; }
        public DbSet<EvaluateView> EvaluateViews { set; get; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>().HasMany(o => o.OrderGoods).WithRequired().HasForeignKey(g => g.OrderId);
            modelBuilder.Entity<Order>().HasMany(o => o.OrderActions).WithRequired().HasForeignKey(g => g.OrderId);
        }
    }
}
