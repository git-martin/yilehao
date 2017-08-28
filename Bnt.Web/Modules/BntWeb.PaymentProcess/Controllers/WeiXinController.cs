using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using Autofac;
using BntWeb.Config.Models;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.Environment;
using BntWeb.Logging;
using BntWeb.MemberBase.Models;
using BntWeb.MemberBase.Services;
using BntWeb.OrderProcess.Models;
using BntWeb.OrderProcess.Services;
using BntWeb.PaymentProcess.Models;
using BntWeb.PaymentProcess.Payments;
using BntWeb.PaymentProcess.Payments.WeiXin;
using BntWeb.PaymentProcess.Services;
using BntWeb.Services;
using BntWeb.Utility.Extensions;
using BntWeb.Wallet.Services;
using BntWeb.WebApi.Models;
using Senparc.Weixin;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.TenPayLibV3;

namespace BntWeb.PaymentProcess.Controllers
{
    public class WeiXinController : Controller
    {
        private readonly IMemberService _memberService;
        private readonly ICurrencyService _currencyService;
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly IWalletService _walletService;
        private readonly UrlHelper _urlHelper;
        private readonly IPublicService _publicService;
        private readonly IConfigService _configService;

        public WeiXinController(IMemberService memberService, ICurrencyService currencyService, IPaymentService paymentService, IOrderService orderService, IWalletService walletService, UrlHelper urlHelper, IPublicService publicService, IConfigService configService)
        {
            _memberService = memberService;
            _currencyService = currencyService;
            _paymentService = paymentService;
            _orderService = orderService;
            _walletService = walletService;
            _urlHelper = urlHelper;
            _publicService = publicService;
            _configService = configService;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public string AppId => _configService.Get<WeiXinConfig>().AppId;

        public string AppSecret => _configService.Get<WeiXinConfig>().AppSecret;

        public string MchId => _configService.Get<WeiXinConfig>().MchId;

        public string Key => _configService.Get<WeiXinConfig>().Key;


        #region JsApi支付

        /// <summary>
        /// 获取用户的OpenId
        /// </summary>
        /// <returns></returns>
        public ActionResult PayOAuth(Guid orderId, int useBalance)
        {
            //获取产品信息
            if (orderId.Equals(Guid.Empty))
                return Content("订单Id不合法！");

            var order = _currencyService.GetSingleById<Order>(orderId);
            if (order == null)
                return Content("订单数据不存在！");

            if (order.OrderStatus != OrderStatus.PendingPayment && order.PayStatus != PayStatus.Unpaid)
                return Content("订单状态不合理，无法支付！");

            var payment = _paymentService.LoadPayment("weixin");
            if (payment == null || !payment.Enabled)
                return Content("支付方式不合法或已停用！");

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
                }

                #endregion
            }

            if (order.PayStatus == PayStatus.Paid)
                return Redirect(_publicService.GetReturnUrl(order.Id));

            var state = $"{orderId}";
            var oathInfo = _currencyService.GetSingleByConditon<UserOAuth>(ua => ua.MemberId == order.MemberId && ua.OAuthType == OAuthType.WeiXin);
            if (oathInfo != null)
            {
                var result = JsApiPayProcess($"|{oathInfo.OAuthId}|", state);
                if (result != null)
                    return result;
                return View("JsApi");
            }

            //需要授权
            var routeParas = new RouteValueDictionary{
                    { "area", PaymentProcessModule.Area},
                    { "controller", "WeiXin"},
                    { "action", "JsApi"}
                };
            var returnUrl = HostConstObject.HostUrl + _urlHelper.RouteUrl(routeParas);
            var url = OAuthApi.GetAuthorizeUrl(AppId, returnUrl, state, OAuthScope.snsapi_userinfo);

            return Redirect(url);
        }

        private ActionResult JsApiPayProcess(string code, string state)
        {
            var needOath = !code.StartsWith("|");

            if (string.IsNullOrEmpty(code))
            {
                return Content("您拒绝了授权！");
            }

            //获取产品信息
            var orderId = state.ToGuid();
            if (orderId.Equals(Guid.Empty))
                return Content("订单Id不合法！");

            var order = _orderService.Load(orderId);
            if (order == null)
                return Content("订单数据不存在！");

            if (order.OrderStatus != OrderStatus.PendingPayment && order.PayStatus != PayStatus.Unpaid)
                return Content("订单状态不合理，无法支付！");

            var payment = _paymentService.LoadPayment("weixin");
            if (payment == null || !payment.Enabled)
                return Content("支付方式不合法或已停用！");

            string openId;
            if (needOath)
            {
                //通过，用code换取access_token
                var openIdResult = OAuthApi.GetAccessToken(AppId, AppSecret, code);
                if (openIdResult.errcode != ReturnCode.请求成功)
                {
                    return Content("错误：" + openIdResult.errmsg);
                }

                openId = openIdResult.openid;
            }
            else
            {
                openId = code.Trim('|');
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

            string timeStamp = "";
            string nonceStr = "";
            string paySign = "";

            //创建支付应答对象
            WxRequestHandler packageReqHandler = new WxRequestHandler(null);
            //初始化
            packageReqHandler.Init();

            timeStamp = TenPayV3Util.GetTimestamp();
            nonceStr = TenPayV3Util.GetNoncestr();

            var routeParas = new RouteValueDictionary{
                    { "area", PaymentProcessModule.Area},
                    { "controller", "Receive"},
                    { "action", "AsyncReturn"},
                    { "paymentCode", "WeiXin"}
                };
            var notifyUrl = HostConstObject.HostUrl + _urlHelper.RouteUrl(routeParas);

            //设置package订单参数
            packageReqHandler.SetParameter("appid", AppId);		                                                //公众账号ID
            packageReqHandler.SetParameter("mch_id", MchId);		                                            //商户号
            packageReqHandler.SetParameter("nonce_str", nonceStr);                                              //随机字符串
            packageReqHandler.SetParameter("body", order.OrderNo);                                                     //商品信息
            packageReqHandler.SetParameter("out_trade_no", payLog.TransactionNo);		                        //商家订单号
            packageReqHandler.SetParameter("total_fee", Convert.ToInt32(payLog.OrderAmount * 100).ToString());	//商品金额,以分为单位(money * 100).ToString()
            packageReqHandler.SetParameter("spbill_create_ip", Request.UserHostAddress);                        //用户的公网ip，不是商户服务器IP
            packageReqHandler.SetParameter("notify_url", notifyUrl);		                                    //接收财付通通知的URL
            packageReqHandler.SetParameter("trade_type", TenPayV3Type.JSAPI.ToString());	                    //交易类型
            packageReqHandler.SetParameter("openid", openId);	                                    //用户的openId

            string sign = packageReqHandler.CreateMd5Sign("key", Key);
            packageReqHandler.SetParameter("sign", sign);	//签名

            string data = packageReqHandler.ParseXML();

#pragma warning disable 618
            var result = TenPayV3.Unifiedorder(data);
#pragma warning restore 618

            var res = XDocument.Parse(result);
            var prepayXml = res.Element("xml").Element("prepay_id");
            if (prepayXml == null)
            {
                return Content(res.Element("xml").Element("return_msg").Value);
            }
            string prepayId = prepayXml.Value;

            //设置支付参数
            WxRequestHandler paySignReqHandler = new WxRequestHandler(null);
            paySignReqHandler.SetParameter("appId", AppId);
            paySignReqHandler.SetParameter("timeStamp", timeStamp);
            paySignReqHandler.SetParameter("nonceStr", nonceStr);
            paySignReqHandler.SetParameter("package", $"prepay_id={prepayId}");
            paySignReqHandler.SetParameter("signType", "MD5");
            paySign = paySignReqHandler.CreateMd5Sign("key", Key);

            ViewData["appId"] = AppId;
            ViewData["timeStamp"] = timeStamp;
            ViewData["nonceStr"] = nonceStr;
            ViewData["package"] = $"prepay_id={prepayId}";
            ViewData["paySign"] = paySign;

            return null;
        }

        public ActionResult JsApi(string code, string state)
        {
            var result = JsApiPayProcess(code, state);
            if (result != null)
                return result;
            return View();
        }

        #endregion
    }
}