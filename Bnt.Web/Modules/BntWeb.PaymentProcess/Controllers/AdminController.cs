using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
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
using BntWeb.Wallet.Models;
using BntWeb.Wallet.Services;
using BntWeb.Web.Extensions;
using PaymentType = BntWeb.PaymentProcess.Models.PaymentType;

namespace BntWeb.PaymentProcess.Controllers
{
    public class AdminController : Controller
    {
        private readonly ICurrencyService _currencyService;
        private readonly IPaymentService _paymentService;
        private readonly IWalletService _walletService;
        private readonly UrlHelper _urlHelper;

        public AdminController(ICurrencyService currencyService, IPaymentService paymentService, IWalletService walletService, UrlHelper urlHelper)
        {
            _currencyService = currencyService;
            _paymentService = paymentService;
            _walletService = walletService;
            _urlHelper = urlHelper;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ConfigPaymentKey })]
        public ActionResult List()
        {
            ViewBag.Payments = _currencyService.GetAll<Payment>();
            return View();
        }



        [HttpPost]
        [AdminAuthorize(PermissionsArray = new[] { Permissions.ConfigPaymentKey })]
        public ActionResult Switch(SwitchViewModel switchPayment)
        {
            var result = new DataJsonResult();
            var payment = _currencyService.GetSingleById<Payment>(switchPayment.Id);
            if (payment != null)
            {
                payment.Enabled = switchPayment.Enabled;
                _currencyService.Update(payment);
            }
            else
            {
                result.ErrorMessage = "支付方式不存在！";
            }

            return Json(result);
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.TransferKey })]
        public ActionResult Transfer(TransferViewModel transfer)
        {
            if (transfer.PaymentCode.Equals("offline", StringComparison.OrdinalIgnoreCase))
            {
                var result = new DataJsonResult();

                if (transfer.ApplyIds != null && transfer.ApplyIds.Count > 0)
                {
                    var crashApplys =
                        transfer.ApplyIds.Select(applyId => _walletService.GetCrashApplyById(applyId))
                            .Where(apply => apply != null && apply.ApplyState == ApplyState.ApplyPassed)
                            .ToList();
                    if (crashApplys.Count > 0)
                    {
                        foreach (var crashApply in crashApplys)
                        {
                            //提交成功
                            crashApply.ApplyState = ApplyState.Transferred;
                            crashApply.TransferTime = DateTime.Now;
                            crashApply.Description = "线下提现成功";
                            _currencyService.Update(crashApply);
                            string error;
                            if (!_walletService.Draw(crashApply.MemberId, WalletType.Cash, crashApply.Money, "提现支出",
                                    out error, null, null, true))
                            {
                                Logger.Operation($"提现转账成功，扣除冻结金额处理失败，TransactionNo：{crashApply.TransactionNo}，原因：{error}",
                                    PaymentProcessModule.Instance, Security.SecurityLevel.Danger);
                            }
                        }
                    }
                    else
                    {
                        result.ErrorMessage = "没有选择任何有效提现申请";
                    }
                }
                else
                {
                    result.ErrorMessage = "没有选择任何提现申请";
                }



                ViewBag.Html = result.ToJson();
            }
            else
            {

                //支付信息
                var payment = _paymentService.LoadPayment(transfer.PaymentCode.ToLower());

                var routeParas = new RouteValueDictionary
                {
                    {"area", PaymentProcessModule.Area},
                    {"controller", "Receive"},
                    {"action", "TransferAsyncReturn"},
                    {"paymentCode", payment.Code}
                };
                var notifyUrl = HostConstObject.HostUrl + _urlHelper.RouteUrl(routeParas);

                if (transfer.ApplyIds != null && transfer.ApplyIds.Count > 0)
                {
                    var crashApplys =
                        transfer.ApplyIds.Select(applyId => _walletService.GetCrashApplyById(applyId))
                            .Where(apply => apply != null && apply.ApplyState == ApplyState.ApplyPassed)
                            .ToList();
                    if (crashApplys.Count > 0)
                    {
                        var withdrawalDispatcher =
                            HostConstObject.Container.ResolveNamed<IWithdrawalDispatcher>(payment.Code.ToLower());
                        var sHtmlText = withdrawalDispatcher.Transfer(crashApplys, payment, notifyUrl);

                        ViewBag.Html = sHtmlText;
                    }
                    else
                    {
                        var result = new DataJsonResult();
                        result.ErrorMessage = "没有选择任何有效提现申请";
                        ViewBag.Html = transfer.PaymentCode.Equals(PaymentType.WeiXin.ToString(),
                            StringComparison.OrdinalIgnoreCase)
                            ? result.ToJson()
                            : "没有选择任何有效提现申请！";
                    }
                }
                else
                {
                    var result = new DataJsonResult();
                    result.ErrorMessage = "没有选择任何提现申请";
                    ViewBag.Html = transfer.PaymentCode.Equals(PaymentType.WeiXin.ToString(),
                        StringComparison.OrdinalIgnoreCase)
                        ? result.ToJson()
                        : "没有选择任何提现申请！";
                }
            }
            return Content(ViewBag.Html);
        }
    }
}