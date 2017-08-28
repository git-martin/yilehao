using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using BntWeb.Data;
using BntWeb.Logistics.Models;

namespace BntWeb.Logistics
{
    public class LogisticsDbContext : BaseDbContext
    {
        public DbSet<Shipping> Shippings { set; get; }
        public DbSet<ShippingArea> ShippingAreas { get; set; }
        public DbSet<ShippingAreaFee> ShippingAreasFees { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}