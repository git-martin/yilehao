/* 
    ======================================================================== 
        File name：        IOrderServiceProxy
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/7/11 10:56:53
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BntWeb.OrderProcess.Models;

namespace BntWeb.OrderProcess.Services
{
    public interface IOrderServiceProxy : IDependency
    {
        /// <summary>
        /// 返回模块Key
        /// </summary>
        /// <returns></returns>
        string ModuleKey();

        /// <summary>
        /// 提交订单
        /// </summary>
        /// <param name="order"></param>
        void AfterSubmitOrder(Order order);

        /// <summary>
        /// 修改订单状态
        /// </summary>
        /// <param name="order"></param>
        /// <param name="oldStatus"></param>
        /// <returns></returns>
        void AfterChangeOrderStatus(Order order, OrderStatus oldStatus);

        /// <summary>
        /// 修改支付状态
        /// </summary>
        /// <param name="order"></param>
        /// <param name="oldStatus"></param>
        /// <returns></returns>
        void AfterChangePayStatus(Order order, PayStatus oldStatus);

        /// <summary>
        /// 修改物流状态
        /// </summary>
        /// <param name="order"></param>
        /// <param name="oldStatus"></param>
        /// <returns></returns>
        void AfterChangeShippingStatus(Order order, ShippingStatus oldStatus);

        /// <summary>
        /// 修改评价状态
        /// </summary>
        /// <param name="order"></param>
        /// <param name="oldEvaluateStatus"></param>
        void AfterChangeEvaluateStatus(Order order, EvaluateStatus oldEvaluateStatus);
    }
}