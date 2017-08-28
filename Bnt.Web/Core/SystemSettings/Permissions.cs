/* 
    ======================================================================== 
        File name：        Permissions
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/5/9 15:34:55
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BntWeb.Security.Permissions;

namespace BntWeb.Core.SystemSettings
{
    public class Permissions : IPermissionProvider
    {
        private static readonly string CategoryKey = SystemSettingsModule.DisplayName;

        public const string ViewFileKey = "Core-SystemSettings-ViewFile";
        public static readonly Permission ViewFile = new Permission { Description = "查看文件", Name = ViewFileKey, Category = CategoryKey };
        public const string UploadFileKey = "Core-SystemSettings-UploadFile";
        public static readonly Permission UploadFile = new Permission { Description = "上传文件", Name = UploadFileKey, Category = CategoryKey };
        public const string DeleteFileKey = "Core-SystemSettings-DeleteFile";
        public static readonly Permission DeleteFile = new Permission { Description = "删除文件", Name = DeleteFileKey, Category = CategoryKey };

        public const string ViewSmsKey = "Core-SystemSettings-ViewSms";
        public static readonly Permission ViewSms = new Permission { Description = "查看短信日志", Name = ViewSmsKey, Category = CategoryKey };

        public const string ViewLogKey = "Core-SystemSettings-ViewLog";
        public static readonly Permission ViewLog = new Permission { Description = "查看日志", Name = ViewLogKey, Category = CategoryKey };

        public const string ViewDistrictKey = "Core-SystemSettings-ViewDistrict";
        public static readonly Permission ViewDistrict = new Permission { Description = "查看行政区", Name = ViewDistrictKey, Category = CategoryKey };
        public const string EditDistrictKey = "Core-SystemSettings-EditDistrict";
        public static readonly Permission EditDistrict = new Permission { Description = "编辑行政区", Name = EditDistrictKey, Category = CategoryKey };

        public const string SoftReleaseKey = "Core-SystemSettings-SoftRelease";
        public static readonly Permission SoftRelease = new Permission { Description = "软件版本", Name = SoftReleaseKey, Category = CategoryKey };

        public const string DbBackupKey = "Core-SystemSettings-DbBackup";
        public static readonly Permission DbBackup = new Permission { Description = "数据库备份", Name = DbBackupKey, Category = CategoryKey };

        public int Position => SystemSettingsModule.Position;

        public string Category => CategoryKey;

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ViewFile,
                UploadFile,
                DeleteFile,
                ViewSms,
                ViewLog,
                ViewDistrict,
                EditDistrict,
                SoftRelease,
                DbBackup
            };
        }
    }
}