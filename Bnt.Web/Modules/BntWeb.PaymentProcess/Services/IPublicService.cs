using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BntWeb.PaymentProcess.Services
{
    public interface IPublicService : IDependency
    {
        /// <summary>
        /// 获取订单支付回调路径
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        string GetReturnUrl(Guid orderId);

    }
}