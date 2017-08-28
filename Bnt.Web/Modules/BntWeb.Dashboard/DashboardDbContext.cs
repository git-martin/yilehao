using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using BntWeb.Core.SystemSettings.Models;
using BntWeb.Dashboard.ViewModels;
using BntWeb.Data;
using BntWeb.MemberBase.Models;
using BntWeb.OrderProcess.Models;

namespace BntWeb.Dashboard
{
    public class DashboardDbContext : BaseDbContext
    {
        public DbSet<Member> Members { get; set; }
        public DbSet<Order> Orders { get; set; }

    }
}