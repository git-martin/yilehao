using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.Environment;
using BntWeb.Logging;
using BntWeb.MemberBase.Services;
using BntWeb.OrderProcess.Models;
using BntWeb.OrderProcess.Services;
using BntWeb.PaymentProcess.Models;
using BntWeb.PaymentProcess.Payments;
using BntWeb.PaymentProcess.Services;
using BntWeb.PaymentProcess.ViewModels;
using BntWeb.Utility.Extensions;
using BntWeb.Wallet.Services;
using BntWeb.WebApi.Models;
namespace BntWeb.PaymentProcess.Controllers
{
    public class AlipayController : Controller
    {
        private readonly IMemberService _memberService;
        private readonly ICurrencyService _currencyService;
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly IWalletService _walletService;
        private readonly UrlHelper _urlHelper;
        private readonly IPublicService _publicService;

        public AlipayController(IMemberService memberService, ICurrencyService currencyService, IPaymentService paymentService, IOrderService orderService, IWalletService walletService, UrlHelper urlHelper, IPublicService publicService)
        {
            _memberService = memberService;
            _currencyService = currencyService;
            _paymentService = paymentService;
            _orderService = orderService;
            _walletService = walletService;
            _urlHelper = urlHelper;
            _publicService = publicService;
            Logger = NullLogger.Instance;
        }
        public ILogger Logger { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Pay(PayViewModel payModel)
        {
            //获取产品信息
            if (payModel.OrderId.Equals(Guid.Empty))
                return Content("订单Id不合法！");
            var order = _orderService.Load(payModel.OrderId);
            if (order == null)
                return Content("订单数据不存在！");
            if (order.OrderStatus != OrderStatus.PendingPayment && order.PayStatus != PayStatus.Unpaid)
                return Content("订单状态不合理，无法支付！");
            var payment = _paymentService.LoadPayment(payModel.PaymentCode);
            if (payment == null || !payment.Enabled)
                return Content("支付方式不合法或已停用！");
            var paymentDispatcher = HostConstObject.Container.ResolveNamed<IPaymentDispatcher>(payment.Code.ToLower());
            if (paymentDispatcher == null)
                return Content("支付方式不合法");
            var routeParas = new RouteValueDictionary{
                    { "area", PaymentProcessModule.Area},
                    { "controller", "Receive"},
                    { "action", "AsyncReturn"},
                    { "paymentCode", payment.Code}
                };
            var notifyUrl = HostConstObject.HostUrl + _urlHelper.RouteUrl(routeParas);
            var routeParas2 = new RouteValueDictionary{
                    { "area", PaymentProcessModule.Area},
                    { "controller", "Receive"},
                    { "action", "SyncReturn"},
                    { "paymentCode", payment.Code},
                    { "orderId", order.Id}
                };
            var returnUrl = HostConstObject.HostUrl + _urlHelper.RouteUrl(routeParas2);
            if (payModel.UseBalance == 1)
            {
                //使用余额付款
                #region
                using (TransactionScope scope = new TransactionScope())
                {
                    var cashWallet = _walletService.GetWalletByMemberId(order.MemberId,
                        Wallet.Models.WalletType.Cash);
                    if (cashWallet != null && cashWallet.Available > 0)
                    {
                        if (cashWallet.Available >= order.PayFee)
                        {
                            string error;
                            if (_walletService.Draw(order.MemberId, Wallet.Models.WalletType.Cash, order.PayFee,
                                "支付订单" + order.OrderNo, out error))
                            {
                                order.BalancePay = order.PayFee;
                                order.OrderStatus = OrderStatus.WaitingForDelivery;
                                order.PayStatus = PayStatus.Paid;
                                order.PayTime = DateTime.Now;
                                order.PaymentName = PaymentType.Balance.Description();
                                _orderService.ChangeOrderStatus(order.Id, order.OrderStatus, order.PayStatus);
                                _currencyService.Update(order);

                                scope.Complete();
                            }
                        }
                        else
                        {
                            string error;
                            if (_walletService.Draw(order.MemberId, Wallet.Models.WalletType.Cash,
                                cashWallet.Available,
                                "支付订单" + order.OrderNo, out error))
                            {
                                order.UnpayFee = order.PayFee - cashWallet.Available;
                                order.BalancePay = cashWallet.Available;
                                _currencyService.Update(order);

                                scope.Complete();
                            }
                        }
                    }
                    else
                    {
                        throw new WebApiInnerException("0003", "可用余额不足");
                    }
                }
                #endregion
            }
            if (order.PayStatus == PayStatus.Paid)
                return Redirect(_publicService.GetReturnUrl(order.Id));

            var payLog = new PayLog
            {
                Id = KeyGenerator.GetGuidKey(),
                TransactionNo = $"{order.OrderNo}{KeyGenerator.GenerateRandom(1000, 1)}",
                OrderId = order.Id,
                OrderNo = order.OrderNo,
                OrderAmount = order.UnpayFee,
                PaymentId = payment.Id,
                PaymentName = payment.Name,
                CreateTime = DateTime.Now,
                LogStatus = LogStatus.Unpaid
            };
            if (!_currencyService.Create(payLog))
                throw new WebApiInnerException("0007", "生成支付流水失败");
            var subject = $"支付订单{order.OrderNo}-{payLog.TransactionNo}";
            var body = string.Join(";", order.OrderGoods.Select(g => g.GoodsName));
            ViewBag.Html = paymentDispatcher.WebPay(subject, body, notifyUrl, returnUrl, payLog, payment);

            return View();
        }
    }
}