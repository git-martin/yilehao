using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml.Linq;
using Autofac;
using BntWeb.Config.Models;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.Environment;
using BntWeb.Logging;
using BntWeb.MemberBase.Models;
using BntWeb.OrderProcess.Models;
using BntWeb.OrderProcess.Services;
using BntWeb.PaymentProcess.Models;
using BntWeb.PaymentProcess.Payments;
using BntWeb.PaymentProcess.Payments.WeiXin;
using BntWeb.PaymentProcess.Services;
using BntWeb.PaymentProcess.ViewModels;
using BntWeb.Services;
using BntWeb.Validation;
using BntWeb.Wallet.Services;
using BntWeb.WebApi.Filters;
using BntWeb.WebApi.Models;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.TenPayLibV3;
using WxPayAPI;

namespace BntWeb.PaymentProcess.ApiControllers
{
    public class ProcessController : BaseApiController
    {
        private readonly ICurrencyService _currencyService;
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly UrlHelper _urlHelper;
        private readonly IWalletService _walletService;
        private readonly IPublicService _publicService;
        private readonly IConfigService _configService;


        public ProcessController(ICurrencyService currencyService, IPaymentService paymentService, IOrderService orderService, UrlHelper urlHelper, IWalletService walletService, IPublicService publicService, IConfigService configService)
        {
            _currencyService = currencyService;
            _paymentService = paymentService;
            _orderService = orderService;
            _urlHelper = urlHelper;
            _walletService = walletService;
            _publicService = publicService;
            _configService = configService;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        /// <summary>
        /// App支付生成签名
        /// </summary>
        /// <param name="paymentCode"></param>
        /// <param name="orderId"></param>
        /// <param name="useBalance">是否使用余额支付，1：使用，0：不使用</param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        [BasicAuthentication]
        public ApiResult SignInfo(string paymentCode, Guid orderId, int useBalance)
        {
            if (orderId.Equals(Guid.Empty))
                throw new WebApiInnerException("0001", "订单Id不合法");

            var order = _orderService.Load(orderId);
            if (order == null)
                throw new WebApiInnerException("0002", "订单数据不存在");

            if (order.OrderStatus != OrderStatus.PendingPayment && order.PayStatus != PayStatus.Unpaid)
                throw new WebApiInnerException("0003", "订单状态不合理，无法支付");

            var payment = _paymentService.LoadPayment(paymentCode);

            if (payment == null || !payment.Enabled)
                throw new WebApiInnerException("0004", "支付方式不合法或已停用");

            var paymentDispatcher = HostConstObject.Container.ResolveNamed<IPaymentDispatcher>(payment.Code.ToLower());
            if (paymentDispatcher == null)
                throw new WebApiInnerException("0005", "支付方式不合法");

            var result = new ApiResult();
            try
            {
                var routeParas = new RouteValueDictionary{
                    { "area", PaymentProcessModule.Area},
                    { "controller", "Receive"},
                    { "action", "AsyncReturn"},
                    { "paymentCode", payment.Code}
                };
                var notifyUrl = HostConstObject.HostUrl + _urlHelper.RouteUrl(routeParas);

                if (payment.Code.Equals("balance"))
                {
                    //使用余额付款
                    #region

                    using (TransactionScope scope = new TransactionScope())
                    {
                        var cashWallet = _walletService.GetWalletByMemberId(order.MemberId,
                            Wallet.Models.WalletType.Cash);
                        if (cashWallet != null && cashWallet.Available >= order.PayFee)
                        {
                            string error;
                            if (_walletService.Draw(order.MemberId, Wallet.Models.WalletType.Cash, order.PayFee,
                                "支付订单" + order.OrderNo, out error))
                            {
                                order.BalancePay = order.PayFee;
                                order.OrderStatus = OrderStatus.WaitingForDelivery;
                                order.PayStatus = PayStatus.Paid;
                                order.PayTime = DateTime.Now;

                                var balancePayment = _paymentService.LoadPayment(PaymentType.Balance.ToString());
                                order.PaymentId = balancePayment.Id;
                                order.PaymentName = balancePayment.Name;
                                _orderService.ChangeOrderStatus(order.Id, order.OrderStatus, order.PayStatus);
                                _currencyService.Update(order);

                                scope.Complete();
                            }
                        }
                        else
                            throw new WebApiInnerException("0008", "账号余额不足");
                    }

                    #endregion
                }
                else if (useBalance == 1)
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
                                if (_walletService.Draw(order.MemberId, Wallet.Models.WalletType.Cash, order.PayFee, "支付订单" + order.OrderNo, out error))
                                {
                                    order.BalancePay = order.PayFee;
                                    order.OrderStatus = OrderStatus.WaitingForDelivery;
                                    order.PayStatus = PayStatus.Paid;
                                    order.PayTime = DateTime.Now;

                                    var balancePayment = _paymentService.LoadPayment(PaymentType.Balance.ToString());
                                    order.PaymentId = balancePayment.Id;
                                    order.PaymentName = balancePayment.Name;
                                    _orderService.ChangeOrderStatus(order.Id, order.OrderStatus, order.PayStatus);
                                    _currencyService.Update(order);

                                    scope.Complete();
                                }
                            }
                            else
                            {
                                string error;
                                if (_walletService.Draw(order.MemberId, Wallet.Models.WalletType.Cash, cashWallet.Available, "支付订单" + order.OrderNo, out error))
                                {
                                    order.UnpayFee = order.PayFee - cashWallet.Available;
                                    order.BalancePay = cashWallet.Available;
                                    _currencyService.Update(order);

                                    scope.Complete();
                                }
                            }

                        }

                    }

                    #endregion
                }

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

                result.SetData(paymentDispatcher.GetSignInfo(subject, body, notifyUrl, payLog, payment));
            }
            catch (WebApiInnerException ex)
            {
                Logger.Error(ex, "获取订单支付签名数据失败");
                throw new WebApiInnerException(ex.ErrorCode, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "获取订单支付签名数据失败");
                throw new WebApiInnerException("0006", "生成签名数据出现异常");
            }

            return result;
        }

        /// <summary>
        /// 微信Pc扫码支付
        /// </summary>
        /// <param name="paymentCode"></param>
        /// <param name="orderId"></param>
        /// <param name="useBalance"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        [BasicAuthentication]
        public ApiResult ScanCodePay(string paymentCode, Guid orderId, int useBalance)
        {
            if (orderId.Equals(Guid.Empty))
                throw new WebApiInnerException("0001", "订单Id不合法");
            var order = _orderService.Load(orderId);
            if (order == null)
                throw new WebApiInnerException("0002", "订单数据不存在");
            if (order.OrderStatus != OrderStatus.PendingPayment && order.PayStatus != PayStatus.Unpaid)
                throw new WebApiInnerException("0003", "订单状态不合理，无法支付");
            var payment = _paymentService.LoadPayment(paymentCode);
            if (payment == null || !payment.Enabled)
                throw new WebApiInnerException("0004", "支付方式不合法或已停用");
            var paymentDispatcher = HostConstObject.Container.ResolveNamed<IPaymentDispatcher>(payment.Code.ToLower());
            if (paymentDispatcher == null)
                throw new WebApiInnerException("0005", "支付方式不合法");
            var result = new ApiResult();
            try
            {
                var routeParas = new RouteValueDictionary{
                    { "area", PaymentProcessModule.Area},
                    { "controller", "Receive"},
                    { "action", "AsyncReturn"},
                    { "paymentCode", payment.Code}
                };
                var notifyUrl = HostConstObject.HostUrl + _urlHelper.RouteUrl(routeParas);
                if (useBalance == 1)
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
                                if (_walletService.Draw(order.MemberId, Wallet.Models.WalletType.Cash, order.PayFee, "支付订单" + order.OrderNo, out error))
                                {
                                    order.BalancePay = order.PayFee;
                                    order.OrderStatus = OrderStatus.WaitingForDelivery;
                                    order.PayStatus = PayStatus.Paid;
                                    order.PayTime = DateTime.Now;

                                    var balancePayment = _paymentService.LoadPayment(PaymentType.Balance.ToString());
                                    order.PaymentId = balancePayment.Id;
                                    order.PaymentName = balancePayment.Name;
                                    _orderService.ChangeOrderStatus(order.Id, order.OrderStatus, order.PayStatus);
                                    _currencyService.Update(order);

                                    scope.Complete();
                                }
                            }
                            else
                            {
                                string error;
                                if (_walletService.Draw(order.MemberId, Wallet.Models.WalletType.Cash, cashWallet.Available, "支付订单" + order.OrderNo, out error))
                                {
                                    order.UnpayFee = order.PayFee - cashWallet.Available;
                                    order.BalancePay = cashWallet.Available;
                                    _currencyService.Update(order);

                                    scope.Complete();
                                }
                            }
                        }
                    }
                    #endregion
                }
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
                result.SetData(paymentDispatcher.WebPay(subject, body, notifyUrl, "", payLog, payment));
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "获取订单支付签名数据失败");
                throw new WebApiInnerException("0006", "生成签名数据出现异常");
            }
            return result;
        }

        /// <summary>
        /// 余额支付
        /// </summary>
        /// <returns></returns>
        [System.Web.Mvc.HttpPost]
        [BasicAuthentication]
        public ApiResult BalancePay(Guid orderId)
        {
            //获取产品信息
            if (orderId.Equals(Guid.Empty))
                throw new WebApiInnerException("0001", "订单Id不合法");

            var order = _currencyService.GetSingleById<Order>(orderId);
            if (order == null)
                throw new WebApiInnerException("0002", "订单数据不存在");

            if (order.OrderStatus != OrderStatus.PendingPayment && order.PayStatus != PayStatus.Unpaid)
                throw new WebApiInnerException("0003", "订单状态不合理，无法支付");

            var payment = _paymentService.LoadPayment("balance");
            if (payment == null || !payment.Enabled)
                throw new WebApiInnerException("0004", "支付方式不合法或已停用");

            //使用余额付款
            #region

            using (TransactionScope scope = new TransactionScope())
            {
                var cashWallet = _walletService.GetWalletByMemberId(order.MemberId,
                    Wallet.Models.WalletType.Cash);
                if (cashWallet != null && cashWallet.Available >= order.PayFee)
                {
                    string error;
                    if (_walletService.Draw(order.MemberId, Wallet.Models.WalletType.Cash, order.PayFee, "支付订单" + order.OrderNo, out error))
                    {
                        order.BalancePay = order.PayFee;
                        order.OrderStatus = OrderStatus.WaitingForDelivery;
                        order.PayStatus = PayStatus.Paid;
                        order.PayTime = DateTime.Now;

                        var balancePayment = _paymentService.LoadPayment(PaymentType.Balance.ToString());
                        order.PaymentId = balancePayment.Id;
                        order.PaymentName = balancePayment.Name;
                        _orderService.ChangeOrderStatus(order.Id, order.OrderStatus, order.PayStatus);
                        _currencyService.Update(order);

                        scope.Complete();
                    }
                }
                else
                    throw new WebApiInnerException("0008", "账号余额不足");
            }

            #endregion
            var result = new ApiResult();
            return result;
        }

        /// <summary>
        /// 微信JsApi支付
        /// </summary>
        /// <param name="payModel"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public ApiResult GetWxPayJsApiParam([FromBody]PayViewModel payModel)
        {
            var orderId = payModel.OrderId;
            if (orderId.Equals(Guid.Empty))
                throw new WebApiInnerException("0001", "订单Id不合法");
            var order = _currencyService.GetSingleById<Order>(orderId);
            if (order == null)
                throw new WebApiInnerException("0002", "订单数据不存在");
            if (order.PayStatus != PayStatus.Unpaid)
                throw new WebApiInnerException("0003", "订单状态不合理，无法支付");
            var payment = _paymentService.LoadPayment(PaymentType.WeiXin.ToString());
            if (payment == null || !payment.Enabled)
                throw new WebApiInnerException("0004", "支付方式不合法或已停用");
            var paymentDispatcher = HostConstObject.Container.ResolveNamed<IPaymentDispatcher>(payment.Code.ToLower());
            if (paymentDispatcher == null)
                throw new WebApiInnerException("0005", "支付方式不合法");

            var oath = _currencyService.GetSingleByConditon<UserOAuth>(ua => ua.MemberId == order.MemberId && ua.OAuthType == OAuthType.WeiXin);
            if (oath == null)
            {
                throw new WebApiInnerException("0008", "未绑定微信号，无法支付");
            }

            var result = new ApiResult();
            try
            {
                var routeParas = new RouteValueDictionary{
                    { "area", PaymentProcessModule.Area},
                    { "controller", "Receive"},
                    { "action", "AsyncReturn"},
                    { "paymentCode", payment.Code}
                };
                var notifyUrl = HostConstObject.HostUrl + _urlHelper.RouteUrl(routeParas);
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
                            if (cashWallet.Available > order.PayFee)
                            {
                                string error;
                                _walletService.Draw(order.MemberId, Wallet.Models.WalletType.Cash, order.PayFee,
                                    "支付订单" + order.OrderNo, out error);
                                if (string.IsNullOrWhiteSpace(error))
                                {
                                    order.BalancePay = order.PayFee;
                                    order.OrderStatus = OrderStatus.WaitingForDelivery;
                                    order.PayStatus = PayStatus.Paid;
                                    order.PayTime = DateTime.Now;

                                    var balancePayment = _paymentService.LoadPayment(PaymentType.Balance.ToString());
                                    order.PaymentId = balancePayment.Id;
                                    order.PaymentName = balancePayment.Name;
                                    _orderService.ChangeOrderStatus(order.Id, order.OrderStatus, order.PayStatus);
                                    _currencyService.Update(order);
                                }
                            }
                            else
                            {
                                string error;
                                _walletService.Draw(order.MemberId, Wallet.Models.WalletType.Cash,
                                    cashWallet.Available,
                                    "支付订单" + order.OrderNo, out error);
                                if (string.IsNullOrWhiteSpace(error))
                                {
                                    order.UnpayFee = order.PayFee - cashWallet.Available;
                                    order.BalancePay = cashWallet.Available;
                                    _currencyService.Update(order);
                                }
                            }
                        }
                        scope.Complete();
                    }
                    #endregion
                }
                var payLog = new PayLog
                {
                    Id = KeyGenerator.GetGuidKey(),
                    TransactionNo = $"{order.OrderNo}{KeyGenerator.GenerateRandom(1000, 1)}",
                    OrderId = order.Id,
                    OrderNo = order.OrderNo,
                    OrderAmount = order.PayFee,//UnpayFee
                    PaymentId = payment.Id,
                    PaymentName = payment.Name,
                    CreateTime = DateTime.Now,
                    LogStatus = LogStatus.Unpaid
                };
                if (!_currencyService.Create(payLog))
                    throw new WebApiInnerException("0007", "生成支付流水失败");

                string timeStamp = "";
                string nonceStr = "";
                string paySign = "";

                var appId = _configService.Get<WeiXinConfig>().AppId;
                var mchId = _configService.Get<WeiXinConfig>().MchId;
                var key = _configService.Get<WeiXinConfig>().Key;

                //创建支付应答对象
                WxRequestHandler packageReqHandler = new WxRequestHandler(null);
                //初始化
                packageReqHandler.Init();

                timeStamp = TenPayV3Util.GetTimestamp();
                nonceStr = TenPayV3Util.GetNoncestr();

                //设置package订单参数
                packageReqHandler.SetParameter("appid", appId); //公众账号ID
                packageReqHandler.SetParameter("mch_id", mchId); //商户号
                packageReqHandler.SetParameter("nonce_str", nonceStr); //随机字符串
                packageReqHandler.SetParameter("body", order.OrderNo); //商品信息
                packageReqHandler.SetParameter("out_trade_no", payLog.TransactionNo); //商家订单号
                packageReqHandler.SetParameter("total_fee", Convert.ToInt32(order.PayFee * 100).ToString());
                //商品金额,以分为单位(money * 100).ToString()
                packageReqHandler.SetParameter("spbill_create_ip", "127.0.0.1"); //用户的公网ip，不是商户服务器IP
                packageReqHandler.SetParameter("notify_url", notifyUrl); //接收财付通通知的URL
                packageReqHandler.SetParameter("trade_type", TenPayV3Type.JSAPI.ToString()); //交易类型
                packageReqHandler.SetParameter("openid", oath.OAuthId); //用户的openId

                string sign = packageReqHandler.CreateMd5Sign("key", key);
                packageReqHandler.SetParameter("sign", sign); //签名

                string data = packageReqHandler.ParseXML();
                var orderResult = TenPayV3.Unifiedorder(data);
                var res = XDocument.Parse(orderResult);
                var prepayXml = res.Element("xml").Element("prepay_id");
                if (prepayXml == null)
                {
                    Logger.Error("生成微信预付单失败:" + res.ToString());
                    throw new WebApiInnerException("0009", "生成微信预付单失败");
                    //res.Element("xml").Element("return_msg").Value;
                }
                string prepayId = prepayXml.Value;
                //设置支付参数
                WxRequestHandler paySignReqHandler = new WxRequestHandler(null);
                paySignReqHandler.SetParameter("appId", appId);
                paySignReqHandler.SetParameter("timeStamp", timeStamp);
                paySignReqHandler.SetParameter("nonceStr", nonceStr);
                paySignReqHandler.SetParameter("package", $"prepay_id={prepayId}");
                paySignReqHandler.SetParameter("signType", "MD5");
                paySign = paySignReqHandler.CreateMd5Sign("key", key);

                result.SetData(new { appId, timeStamp, nonceStr, package = $"prepay_id={prepayId}", paySign, signType = "MD5", orderid = order.Id, orderType = order.ModuleKey, orderStatus = order.OrderStatus, payStatus = order.PayStatus });
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "获取订单支付签名数据失败");
                throw new WebApiInnerException("0006", "生成签名数据出现异常");
            }
            return result;
        }

    }
}