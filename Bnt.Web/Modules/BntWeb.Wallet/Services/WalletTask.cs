/* 
    ======================================================================== 
        File name：        OrderProcessTask
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/8/26 20:20:25
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/

using System;
using BntWeb.Config.Models;
using BntWeb.Logging;
using BntWeb.Services;
using BntWeb.Task;

namespace BntWeb.Wallet.Services
{
    public class WalletTask : IBackgroundTask
    {
        private readonly IWalletService _walletService;
        private readonly IConfigService _configService;

        public WalletTask(IWalletService walletService, IConfigService configService)
        {
            _walletService = walletService;
            _configService = configService;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public void Sweep()
        {
            var interval = _configService.Get<SystemConfig>().CrashApplyOutTime;
            //取消超时未付款订单
            var outTime = DateTime.Now.AddHours(-interval);

            var count = _walletService.CancelCrashApply(outTime);
            if (count > 0)
                Logger.Warning("取消超时未处理提现申请{0}条", count);
        }
    }
}