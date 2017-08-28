/* 
    ======================================================================== 
        File name：        ClearLogsTask
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/6/3 9:36:01
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
using BntWeb.Logging;
using BntWeb.Task;

namespace BntWeb.Core.SystemSettings.Services
{
    public class ClearLogsTask : IBackgroundTask
    {
        private readonly ICurrencyService _currencyService;

        public ClearLogsTask(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public void Sweep()
        {
            //清理半年前的日志
            var outTime = DateTime.Now.AddMonths(-6);
            var count = _currencyService.DeleteByConditon<SystemLog>(l => l.CreateTime < outTime);
            if (count > 0)
                Logger.Warning("清理数据库业务日志{0}条", count);
        }
    }
}