/* 
    ======================================================================== 
        File name：        ClearFilesTask
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/6/13 9:36:01
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BntWeb.Core.SystemSettings.Models;
using BntWeb.Data.Services;
using BntWeb.FileSystems.Media;
using BntWeb.Logging;
using BntWeb.Task;

namespace BntWeb.Core.SystemSettings.Services
{
    public class ClearFilesTask : IBackgroundTask
    {
        private readonly IFileService _fileService;

        public ClearFilesTask(IFileService fileService)
        {
            _fileService = fileService;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public void Sweep()
        {
            //清理2天内未被使用的私有文件
            var outTime = DateTime.Now.AddDays(-2);
            var count = _fileService.DeleteFilesForTask(outTime);
            if (count > 0)
                Logger.Warning("删除未引用的无效文件{0}个", count);
        }
    }
}