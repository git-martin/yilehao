using System;
using System.Collections.Generic;
using System.Web;
using BntWeb.PaymentProcess.Models;
using BntWeb.Wallet.Models;

namespace BntWeb.PaymentProcess.Payments
{
    public interface IWithdrawalDispatcher
    {
        /// <summary>
        /// 批量转账
        /// </summary>
        /// <param name="applys"></param>
        /// <param name="payment"></param>
        /// <param name="notifyUrl"></param>
        /// <returns></returns>
        string Transfer(List<CrashApply> applys, Payment payment, string notifyUrl);

        /// <summary>
        /// 异步回调
        /// </summary>
        string AsyncReturn(HttpRequestBase request);
    }
}