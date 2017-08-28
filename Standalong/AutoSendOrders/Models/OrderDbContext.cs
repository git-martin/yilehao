using System.Data.Entity;
using MySql.Data.Entity;

namespace AutoSendOrders.Models
{

    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class OrderDbContext:DbContext
    {
        public OrderDbContext() : base("name=martindb")
        {
            Configuration.LazyLoadingEnabled = false;
            //不生成@p__0 IS NULL 语句
            Configuration.UseDatabaseNullSemantics = true;
            Database.SetInitializer<OrderDbContext>(null);
            Configuration.ProxyCreationEnabled = false;
        }
        public DbSet<Order> Orders { get; set;}
        public DbSet<OrderGoods> OrderGoods { get; set;}
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Order>().HasMany(o => o.OrderGoods).WithRequired().HasForeignKey(g => g.OrderId);
        }
    }
}
