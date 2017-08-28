/* 
    ======================================================================== 
        File name：        StorageFileService
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/6/12 11:46:34
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using BntWeb.Data;
using BntWeb.FileSystems.Media;
using BntWeb.Utility.Extensions;
using LinqKit;

namespace BntWeb.Core.SystemSettings.Services
{
    public class StorageFileService : IStorageFileService
    {
        public IList<StorageFileRelation> GetFileRelations(Guid sourceId, string moduleKey, string sourceType = "")
        {
            using (var dbContext = new SystemSettingsDbContext())
            {
                var sourceTypeIsNull = string.IsNullOrWhiteSpace(sourceType);
                return dbContext.StorageFileRelations.AsExpandable().Where(
                    r => r.SourceId == sourceId && r.ModuleKey.Equals(moduleKey, StringComparison.OrdinalIgnoreCase) && (sourceTypeIsNull || r.SourceType.Equals(sourceType, StringComparison.OrdinalIgnoreCase))).OrderBy(r => r.Sort)
                    .ToList();
            }
        }

        public IList<StorageFile> GetFiles(Guid sourceId, string moduleKey, string sourceType = "")
        {
            using (var dbContext = new SystemSettingsDbContext())
            {
                var sourceTypeIsNull = string.IsNullOrWhiteSpace(sourceType);
                return dbContext.StorageFileRelations.AsExpandable().Where(
                    r => r.SourceId == sourceId && r.ModuleKey.Equals(moduleKey, StringComparison.OrdinalIgnoreCase) && (sourceTypeIsNull || r.SourceType.Equals(sourceType, StringComparison.OrdinalIgnoreCase))).OrderBy(r => r.Sort)
                    .Select(r => r.File)
                    .ToList();
            }
        }

        public bool AssociateFile(Guid sourceId, string moduleKey, string moduleName, Guid fileId, string sourceType = "", int sort = 0)
        {
            if (fileId.Equals(Guid.Empty)) return false;

            using (var dbContext = new SystemSettingsDbContext())
            {
                dbContext.StorageFileRelations.Add(new StorageFileRelation
                {
                    Id = KeyGenerator.GetGuidKey(),
                    SourceId = sourceId,
                    FileId = fileId,
                    ModuleKey = moduleKey,
                    ModuleName = moduleName,
                    SourceType = sourceType,
                    CreateTime = DateTime.Now,
                    Sort = sort
                });

                return dbContext.SaveChanges() > 0;
            }
        }

        public bool DisassociateFile(Guid sourceId, string moduleKey, Guid fileId, string sourceType = "")
        {
            if (fileId.Equals(Guid.Empty)) return false;

            using (var dbContext = new SystemSettingsDbContext())
            {
                var sourceTypeIsNull = string.IsNullOrWhiteSpace(sourceType);
                var entitys = dbContext.StorageFileRelations.Where(r => r.SourceId == sourceId && r.ModuleKey.Equals(moduleKey, StringComparison.OrdinalIgnoreCase) && r.FileId == fileId && (sourceTypeIsNull || r.SourceType.Equals(sourceType, StringComparison.OrdinalIgnoreCase))).ToList();
                entitys.ForEach(m => dbContext.Entry(m).State = EntityState.Deleted);

                return dbContext.SaveChanges() > 0;
            }
        }

        public bool DisassociateFile(Guid sourceId, string moduleKey, string sourceType = "")
        {
            using (var dbContext = new SystemSettingsDbContext())
            {
                var sourceTypeIsNull = string.IsNullOrWhiteSpace(sourceType);
                var entitys = dbContext.StorageFileRelations.Where(r => r.SourceId == sourceId && r.ModuleKey.Equals(moduleKey, StringComparison.OrdinalIgnoreCase) && (sourceTypeIsNull || r.SourceType.Equals(sourceType, StringComparison.OrdinalIgnoreCase))).ToList();
                entitys.ForEach(m => dbContext.Entry(m).State = EntityState.Deleted);

                return dbContext.SaveChanges() > 0;
            }
        }
        public bool ReplaceFile(Guid sourceId, string moduleKey, string moduleName, Guid fileId, string sourceType = "")
        {
            DisassociateFile(sourceId, moduleKey, sourceType);
            return AssociateFile(sourceId, moduleKey, moduleName, fileId, sourceType);
        }

        public void ReplaceFile(Guid sourceId, string moduleKey, string moduleName, Guid[] fileIds, string sourceType = "")
        {
            DisassociateFile(sourceId, moduleKey, sourceType);
            int sort = 0;
            foreach (var fileId in fileIds)
            {
                AssociateFile(sourceId, moduleKey, moduleName, fileId, sourceType, sort++);
            }
        }

        public void ReplaceFile(Guid sourceId, string moduleKey, string moduleName, string fileIds, string sourceType = "")
        {
            var ids = fileIds?.Trim().Split(',').Select(i => i.ToGuid()).ToArray() ?? new Guid[0];
            ReplaceFile(sourceId, moduleKey, moduleName, ids, sourceType);
        }


        public bool SetFileSort(Guid sourceId, string moduleKey, Guid fileId, int sort, string sourceType = "")
        {
            if (fileId.Equals(Guid.Empty)) return false;

            using (var dbContext = new SystemSettingsDbContext())
            {
                var sourceTypeIsNull = string.IsNullOrWhiteSpace(sourceType);
                var entitys = dbContext.StorageFileRelations.Where(r => r.SourceId == sourceId && r.ModuleKey.Equals(moduleKey, StringComparison.OrdinalIgnoreCase) && r.FileId == fileId && (sourceTypeIsNull || r.SourceType.Equals(sourceType, StringComparison.OrdinalIgnoreCase))).ToList();
                entitys.ForEach(m => m.Sort = sort);

                return dbContext.SaveChanges() > 0;
            }
        }
        public bool DeleteBySourceType(string sourceType)
        {
            if (string.IsNullOrWhiteSpace(sourceType)) return false;

            using (var dbContext = new SystemSettingsDbContext())
            {
                var entitys = dbContext.StorageFileRelations.Where(r => r.SourceType.Equals(sourceType, StringComparison.OrdinalIgnoreCase)).ToList();
                entitys.ForEach(m => dbContext.Entry(m).State = EntityState.Deleted);

                return dbContext.SaveChanges() > 0;
            }
        }
    }
}