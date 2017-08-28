/* 
    ======================================================================== 
        File name：        BalancePayment
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/7/13 14:51:12
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

namespace BntWeb.PaymentProcess.Payments.Balance
{
    public class BalancePayment : IPaymentDispatcher
    {
        public string SyncReturn(HttpRequestBase request)
        {
            return string.Empty;
        }

        public string AsyncReturn(HttpRequestBase request)
        {
            return string.Empty;
        }

        public string GetSignInfo(string subject, string body, string notifyUrl, PayLog payLog, Payment payment, Dictionary<string, string> param = null)
        {
            return string.Empty;
        }

        public string H5Pay(string subject, string body, string notifyUrl, string returnUrl, PayLog payLog, Payment payment,
            Dictionary<string, string> param = null)
        {
            return string.Empty;
        }

        public string WebPay(string subject, string body, string notifyUrl, string returnUrl, PayLog payLog, Payment payment,
            Dictionary<string, string> param = null)
        {
            return string.Empty;
        }
    }
}