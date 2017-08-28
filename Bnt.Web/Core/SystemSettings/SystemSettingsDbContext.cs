/* 
    ======================================================================== 
        File name：        SystemSettingsDbContext
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/5/27 10:05:09
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using BntWeb.Core.SystemSettings.Models;
using BntWeb.Data;
using BntWeb.FileSystems.Media;

namespace BntWeb.Core.SystemSettings
{
    public class SystemSettingsDbContext : BaseDbContext
    {
        public DbSet<SmsLog> SmsLogs { get; set; }

        public DbSet<SystemLog> Logs { get; set; }
        public DbSet<StorageFile> StorageFiles { get; set; }
        public DbSet<StorageFileRelation> StorageFileRelations { get; set; }

        static SystemSettingsDbContext()
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<SystemSettingsDbContext>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
    }
}