using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BntWeb.Environment;
using BntWeb.FileSystems.Media;
using BntWeb.Wallet.Models;

namespace BntWeb.Wallet.ApiModels
{
    public class CrashapplyModel
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }

        public string RealName { get; set; }

        /// <summary>
        /// 提现金额
        /// </summary>
        public decimal Money { get; set; }

        public PaymentType PaymentType { get; set; } = PaymentType.Alipay;

        public string SmsVerifyCode { get; set; }
    }
}