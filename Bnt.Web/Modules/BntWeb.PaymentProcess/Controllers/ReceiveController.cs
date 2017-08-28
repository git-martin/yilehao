using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Autofac;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.Environment;
using BntWeb.Logging;
using BntWeb.Mvc;
using BntWeb.PaymentProcess.Models;
using BntWeb.PaymentProcess.Payments;
using BntWeb.PaymentProcess.Services;
using BntWeb.PaymentProcess.ViewModels;
using BntWeb.Security;
using BntWeb.Utility.Extensions;
using BntWeb.Web.Extensions;

namespace BntWeb.PaymentProcess.Controllers
{
    public class ReceiveController : Controller
    {
        private readonly ICurrencyService _currencyService;
        private readonly IPublicService _publicService;

        public ReceiveController(ICurrencyService currencyService, IPublicService publicService)
        {
            _currencyService = currencyService;
            _publicService = publicService;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public ActionResult SyncReturn(string paymentCode,Guid orderId)
        {
            return Redirect(_publicService.GetReturnUrl(orderId));
            //var paymentDispatcher = HostConstObject.Container.ResolveNamed<IPaymentDispatcher>(paymentCode.ToLower());
            //var result = paymentDispatcher.SyncReturn(Request);
            //return Json(result,JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public string AsyncReturn(string paymentCode)
        {
            var paymentDispatcher = HostConstObject.Container.ResolveNamed<IPaymentDispatcher>(paymentCode.ToLower());
            return paymentDispatcher.AsyncReturn(Request);
        }


        [HttpPost]
        public string TransferAsyncReturn(string paymentCode)
        {
            var withdrawalDispatcher = HostConstObject.Container.ResolveNamed<IWithdrawalDispatcher>(paymentCode.ToLower());
            return withdrawalDispatcher.AsyncReturn(Request);
        }

    }
}