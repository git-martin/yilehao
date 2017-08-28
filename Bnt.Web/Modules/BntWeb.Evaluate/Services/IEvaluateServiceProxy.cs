using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BntWeb.Evaluate.Services
{
    public interface IEvaluateServiceProxy : IDependency
    {
        /// <summary>
        /// 返回模块Key
        /// </summary>
        /// <returns></returns>
        string ModuleKey();

        /// <summary>
        /// 订单评价后修改订单评价状态
        /// </summary>
        /// <param name="orderId"></param>
        void AfterCreateOrderEvaluates(Guid orderId);

        /// <summary>
        /// 订单评价回复后修改订单评价状态
        /// </summary>
        /// <param name="orderId"></param>
        void AfterReplayOrderEvaluates(Guid orderId);

    }
}