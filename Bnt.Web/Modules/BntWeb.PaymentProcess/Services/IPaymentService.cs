/* 
    ======================================================================== 
        File name：        IPaymentService
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/7/15 11:45:56
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BntWeb.PaymentProcess.Models;

namespace BntWeb.PaymentProcess.Services
{
    public interface IPaymentService : IDependency
    {
        /// <summary>
        /// 根据支付Id获取支付方式数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Payment LoadPayment(Guid id);

        /// <summary>
        /// 根据支付Code获取支付方式数据
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        Payment LoadPayment(string code);

        /// <summary>
        /// 保存并更新缓存
        /// </summary>
        /// <param name="payment"></param>
        /// <returns></returns>
        bool SavePayment(Payment payment);
    }
}