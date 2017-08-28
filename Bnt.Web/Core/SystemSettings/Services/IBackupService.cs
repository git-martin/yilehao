/* 
    ======================================================================== 
        File name：        IBackupService
        Module:                
        Author：            罗嗣宝
        Create Time：    2017/2/9 9:45:55
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BntWeb.Core.SystemSettings.Models;

namespace BntWeb.Core.SystemSettings.Services
{
    public interface IBackupService : ISingletonDependency
    {
        bool IsBusy { get; set; }

        /// <summary>
        /// 数据库备份
        /// </summary>
        void Backup(BackupInfo backupInfo);
    }
}
