using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BntWeb.Data.Services;
using BntWeb.OrderProcess.Models;

namespace BntWeb.PaymentProcess.Services
{
    public class PublicService : IPublicService
    {
        private static ICurrencyService _currencyService;

        public PublicService(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        /// <summary>
        /// 获取订单支付回调路径
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public string GetReturnUrl(Guid orderId)
        {
            return "/htmls/success.html";
            //var order = _currencyService.GetSingleById<Order>(orderId);
            //if (order.SourceType.Equals("Recharge"))//充值订单
            //    return "/Html/Member/topup3.html";
            //else //普通订单
            //    return "/Html/Pay2.html?id=" + order.Id;
        }

    }
}