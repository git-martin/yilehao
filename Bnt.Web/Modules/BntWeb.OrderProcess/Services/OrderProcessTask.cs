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
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BntWeb.FileSystems.Media;
using BntWeb.Logging;
using BntWeb.Task;

namespace BntWeb.OrderProcess.Services
{
    public class OrderProcessTask : IBackgroundTask
    {
        private readonly IOrderService _orderService;

        public OrderProcessTask(IOrderService orderService)
        {
            _orderService = orderService;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public void Sweep()
        {
            //取消超时未付款订单
            var outTime = DateTime.Now.AddMinutes(-30); //半小时

            var count = _orderService.CancelTimeOutOrder(outTime);
            if (count > 0)
                Logger.Warning("取消超时未付款订单{0}条", count);

            //自动确认收货
            outTime = DateTime.Now.AddDays(-14); //14天
            count = _orderService.CompleteTimeOutOrder(outTime);
            if (count > 0)
                Logger.Warning("完成超时确认收货订单{0}条", count);
        }
    }
}