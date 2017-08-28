/* 
    ======================================================================== 
        File name：        IPayment
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/7/13 11:43:25
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BntWeb.Config.Models;
using BntWeb.OrderProcess.Models;
using BntWeb.PaymentProcess.Models;

namespace BntWeb.PaymentProcess.Payments
{
    public interface IPaymentDispatcher
    {
        /// <summary>
        /// 同步回调
        /// </summary>
        string SyncReturn(HttpRequestBase request);

        /// <summary>
        /// 异步回调
        /// </summary>
        string AsyncReturn(HttpRequestBase request);

        /// <summary>
        /// 生成订单支付签名数据
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="notifyUrl">异步回调地址</param>
        /// <param name="payLog"></param>
        /// <param name="payment"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        string GetSignInfo(string subject, string body, string notifyUrl, PayLog payLog, Payment payment, Dictionary<string, string> param = null);

        /// <summary>
        /// H5页面支付
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="notifyUrl">异步回调地址</param>
        /// <param name="returnUrl">同步回调地址</param>
        /// <param name="payLog"></param>
        /// <param name="payment"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        string H5Pay(string subject, string body, string notifyUrl, string returnUrl, PayLog payLog, Payment payment, Dictionary<string, string> param = null);
        
        /// <summary>
        /// 网站Pc支付
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="notifyUrl"></param>
        /// <param name="returnUrl"></param>
        /// <param name="payLog"></param>
        /// <param name="payment"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        string WebPay(string subject, string body, string notifyUrl, string returnUrl, PayLog payLog, Payment payment, Dictionary<string, string> param = null);
    }
}