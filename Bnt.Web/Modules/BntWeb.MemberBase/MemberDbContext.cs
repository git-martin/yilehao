/* 
    ======================================================================== 
        File name：        MemberDbContext
        Module:                
        Author：            Luce
        Create Time：    2016/6/7 13:56:43
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/

using System.Data.Entity;
using BntWeb.Data;
using BntWeb.MemberBase.Models;
using BntWeb.Security.Identity;

namespace BntWeb.MemberBase
{
    public class MemberDbContext : BaseDbContext
    {
        public DbSet<Member> Members { get; set; }
        public DbSet<MemberAddress> MemberAddresses { get; set; }

        public DbSet<MemberExtension> MemberExtensions { get; set; }

        public DbSet<MemberFull> MembersFull { set; get; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
