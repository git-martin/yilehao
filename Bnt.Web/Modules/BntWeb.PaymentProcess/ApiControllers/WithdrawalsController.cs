using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Routing;
using Autofac;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.Environment;
using BntWeb.Logging;
using BntWeb.MemberBase.Models;
using BntWeb.OrderProcess.Models;
using BntWeb.OrderProcess.Services;
using BntWeb.PaymentProcess.Models;
using BntWeb.PaymentProcess.Payments;
using BntWeb.PaymentProcess.Services;
using BntWeb.Validation;
using BntWeb.WebApi.Filters;
using BntWeb.WebApi.Models;
using WxPayAPI;

namespace BntWeb.PaymentProcess.ApiControllers
{
    public class WithdrawalsController : BaseApiController
    {
        private readonly ICurrencyService _currencyService;
        private readonly IPaymentService _paymentService;

        public WithdrawalsController(ICurrencyService currencyService, IPaymentService paymentService)
        {
            _currencyService = currencyService;
            _paymentService = paymentService;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        /// <summary>
        /// 获取提现支付方式
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [BasicAuthentication]
        public ApiResult GetWithdrawalsPaymentType()
        {
            var result = new ApiResult();
            var oauth = _currencyService.GetSingleByConditon<UserOAuth>(
                    o => o.OAuthType == OAuthType.WeiXin && o.MemberId == AuthorizedUser.Id);

            var wxPayment = _paymentService.LoadPayment("weixin");
            var alipayPayment = _paymentService.LoadPayment("alipay");

            var data = new
            {
                WeiXin = oauth != null && wxPayment != null && wxPayment.Enabled,
                Alipay = alipayPayment != null && alipayPayment.Enabled
            };

            result.SetData(data);

            return result;
        }
    }
}