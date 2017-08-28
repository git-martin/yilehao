using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using BntWeb.Data;
using BntWeb.FileSystems.Media;

namespace BntWeb.Evaluate
{
    public class EvaluateDbContext : BaseDbContext
    {
        public DbSet<Models.Evaluate> Evaluate { get; set; }

        public DbSet<StorageFile> StorageFiles { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Models.Evaluate>().HasMany(c => c.Files).WithMany().Map(m =>
            {
                m.ToTable(KeyGenerator.TablePrefix + "System_File_Relations");
                m.MapLeftKey("SourceId");
                m.MapRightKey("FileId");
            });
        }
    }
}