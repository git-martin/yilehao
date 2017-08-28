
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using BntWeb.Data;
using BntWeb.MemberBase.Models;

namespace BntWeb.SystemMessage
{
    
    public class SystemMessageDbContext : BaseDbContext
    {
        public DbSet<Models.SystemMessage> SystemMessages { get; set; }


        public DbSet<Models.SystemMessageReciever> SystemMessageRecievers { get; set; }

    }
}
