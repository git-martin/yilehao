/* 
    ======================================================================== 
        File name：        CommentDbContext
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/6/17 16:14:50
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using BntWeb.Data;
using BntWeb.FileSystems.Media;
using BntWeb.MemberBase.Models;

namespace BntWeb.Comment
{
    public class CommentDbContext : BaseDbContext
    {
        public DbSet<Models.Comment> Comments { get; set; }

        public DbSet<StorageFile> StorageFiles { get; set; }

        public DbSet<Member> Members { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Models.Comment>().HasMany(c => c.ChildComments).WithOptional().HasForeignKey(c => c.ParentId);
            
            modelBuilder.Entity<Models.Comment>().HasMany(c => c.Files).WithMany().Map(m =>
            {
                m.ToTable(KeyGenerator.TablePrefix + "System_File_Relations");
                m.MapLeftKey("SourceId");
                m.MapRightKey("FileId");
            });
        }
    }
}