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
using System.Text;
using System.Transactions;
using System.Web;
using System.Web.Http;
using System.Xml.Linq;
using Autofac;
using BntWeb.Config.Models;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.Environment;
using BntWeb.Logging;
using BntWeb.OrderProcess.Models;
using BntWeb.OrderProcess.Services;
using BntWeb.PaymentProcess.Models;
using BntWeb.PaymentProcess.Services;
using BntWeb.Services;
using BntWeb.SystemMessage.Models;
using BntWeb.SystemMessage.Services;
using BntWeb.Utility.Extensions;
using BntWeb.Web.Extensions;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.TenPayLibV3;
using WxPayAPI;

namespace BntWeb.PaymentProcess.Payments.WeiXin
{
    public class WeiXinPayment : IPaymentDispatcher
    {
        private readonly IPaymentService _paymentService;
        private readonly ICurrencyService _currencyService;
        private readonly ISystemMessageService _systemMessageService;
        private readonly IOrderService _orderService;
        private readonly IConfigService _configService;

        public WeiXinPayment(IPaymentService paymentService, ICurrencyService currencyService, ISystemMessageService systemMessageService, IOrderService orderService, IConfigService configService)
        {
            _paymentService = paymentService;
            _currencyService = currencyService;
            _systemMessageService = systemMessageService;
            _orderService = orderService;
            _configService = configService;

            Logger = NullLogger.Instance;
        }
        public ILogger Logger { get; set; }

        public string SyncReturn(HttpRequestBase request)
        {
            var payment = _paymentService.LoadPayment(PaymentType.WeiXin.ToString());
            var config = _configService.Get<WeiXinConfig>();

            WxPayData notifyData = GetNotifyData(request, config.Key);

            //检查支付结果中transaction_id是否存在
            if (!notifyData.IsSet("transaction_id"))
            {
                //若transaction_id不存在，则立即返回结果给微信支付后台
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "FAIL");
                res.SetValue("return_msg", "支付结果中微信订单号不存在");
                Log.Error(this.GetType().ToString(), "The Pay result is error : " + res.ToXml());
                return "支付结果中微信订单号不存在";
            }

            string transaction_id = notifyData.GetValue("transaction_id").ToString();

            string out_trade_no = notifyData.GetValue("out_trade_no").ToString();

            //查询订单，判断订单真实性
            if (!QueryOrder(transaction_id))
            {
                //若订单查询失败，则立即返回结果给微信支付后台
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "FAIL");
                res.SetValue("return_msg", "订单查询失败");
                Log.Error(this.GetType().ToString(), "Order query failure : " + res.ToXml());
                return "订单查询失败";
            }
            //查询订单成功
            else
            {
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "SUCCESS");
                res.SetValue("return_msg", "OK");
                Log.Info(this.GetType().ToString(), "order query success : " + res.ToXml());

                PayLog payLog = _currencyService.GetSingleByConditon<PayLog>(o => o.TransactionNo == out_trade_no);
                if (payLog != null)
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        payLog.PayTime = DateTime.Now;
                        payLog.LogStatus = LogStatus.Paid;
                        _currencyService.Update(payLog);
                        Order order = _currencyService.GetSingleByConditon<Order>(o => o.Id == payLog.OrderId);
                        order.PayStatus = PayStatus.Paid;
                        order.PayTime = DateTime.Now;
                        order.PaymentId = payment.Id;
                        order.PaymentName = payment.Name;
                        order.OrderStatus = OrderStatus.WaitingForDelivery;
                        _orderService.ChangeOrderStatus(order.Id, order.OrderStatus, order.PayStatus);
                        _currencyService.Update(order);

                        var orderAction = new OrderAction
                        {
                            Id = KeyGenerator.GetGuidKey(),
                            OrderId = order.Id,
                            Memo = "微信支付",
                            CreateTime = DateTime.Now,
                            OrderStatus = order.OrderStatus,
                            PayStatus = order.PayStatus,
                            ShippingStatus = order.ShippingStatus,
                            EvaluateStatus = order.EvaluateStatus,
                            UserId = order.MemberId,
                            UserName = order.MemberName
                        };
                        _currencyService.Create(orderAction);
                        //提交
                        scope.Complete();
                    }
                }

                return "OK";
            }
        }

        public string AsyncReturn(HttpRequestBase request)
        {
            var payment = _paymentService.LoadPayment(PaymentType.WeiXin.ToString());
            var config = _configService.Get<WeiXinConfig>();

            Logger.Information("进入微信支付成功异步通知方法");
            WxPayData notifyData = GetNotifyData(request, config.Key);

            //检查支付结果中transaction_id是否存在
            if (!notifyData.IsSet("transaction_id"))
            {
                //若transaction_id不存在，则立即返回结果给微信支付后台
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "FAIL");
                res.SetValue("return_msg", "支付结果中微信订单号不存在");
                Log.Error(this.GetType().ToString(), "The Pay result is error : " + res.ToXml());
                return "支付结果中微信订单号不存在";
            }

            string transaction_id = notifyData.GetValue("transaction_id").ToString();

            string out_trade_no = notifyData.GetValue("out_trade_no").ToString();
            Logger.Information("正在异步操作交易单号：" + transaction_id);
            //查询订单，判断订单真实性
            if (!QueryOrder(transaction_id))
            {
                //若订单查询失败，则立即返回结果给微信支付后台
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "FAIL");
                res.SetValue("return_msg", "订单查询失败");
                Log.Error(this.GetType().ToString(), "Order query failure : " + res.ToXml());
                Logger.Warning("订单查询失败：" + out_trade_no);
                return "订单查询失败";
            }
            //查询订单成功
            else
            {
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "SUCCESS");
                res.SetValue("return_msg", "OK");
                Log.Info(this.GetType().ToString(), "order query success : " + res.ToXml());
                Logger.Warning("订单查询成功: " + res.ToXml());
                PayLog payLog = _currencyService.GetSingleByConditon<PayLog>(o => o.TransactionNo == out_trade_no);
                if (payLog != null)
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        payLog.PayTime = DateTime.Now;
                        payLog.LogStatus = LogStatus.Paid;
                        _currencyService.Update(payLog);
                        Order order = _currencyService.GetSingleByConditon<Order>(o => o.Id == payLog.OrderId);
                        order.PayStatus = PayStatus.Paid;
                        order.PayTime = DateTime.Now;
                        order.PaymentId = payment.Id;
                        order.PaymentName = payment.Name;
                        order.OrderStatus = OrderStatus.WaitingForDelivery;
                        _orderService.ChangeOrderStatus(order.Id, order.OrderStatus, order.PayStatus);
                        _currencyService.Update(order);

                        var orderAction = new OrderAction
                        {
                            Id = KeyGenerator.GetGuidKey(),
                            OrderId = order.Id,
                            Memo = "微信支付",
                            CreateTime = DateTime.Now,
                            OrderStatus = order.OrderStatus,
                            PayStatus = order.PayStatus,
                            ShippingStatus = order.ShippingStatus,
                            EvaluateStatus = order.EvaluateStatus,
                            UserId = order.MemberId,
                            UserName = order.MemberName
                        };
                        _currencyService.Create(orderAction);
                        //提交
                        scope.Complete();
                    }

                }
                Logger.Information("SUCCESS");
                return "SUCCESS";
            }
        }

        public string GetSignInfo(string subject, string body, string notifyUrl, PayLog payLog, Payment payment, Dictionary<string, string> param = null)
        {
            var config = _configService.Get<WeiXinConfig>();

            WxPayData data = new WxPayData();
            data.SetValue("body", body);
            data.SetValue("out_trade_no", payLog.TransactionNo);
            data.SetValue("total_fee", Convert.ToInt32(payLog.OrderAmount * 100));
            data.SetValue("time_start", DateTime.Now.ToString("yyyyMMddHHmmss"));
            data.SetValue("time_expire", DateTime.Now.AddMinutes(10).ToString("yyyyMMddHHmmss"));
            data.SetValue("goods_tag", body);
            data.SetValue("trade_type", "APP");
            data.SetValue("nonce_str", WxPayApi.GenerateNonceStr());
            data.SetValue("notify_url", notifyUrl);

            WeiXinPayConfig weiXinPayConfig = new WeiXinPayConfig() { APPID = config.AppId, MCHID = config.MchId, KEY = config.Key };
            WxPayData result = WxPayApi.UnifiedOrder(data, 6, weiXinPayConfig);

            WxPayData reSignWxPayData = new WxPayData();
            reSignWxPayData.SetValue("appid", result.GetValue("appid"));
            reSignWxPayData.SetValue("partnerid", result.GetValue("mch_id"));
            reSignWxPayData.SetValue("prepayid", result.GetValue("prepay_id"));
            reSignWxPayData.SetValue("noncestr", result.GetValue("nonce_str"));
            reSignWxPayData.SetValue("timestamp", WxPayApi.GenerateTimeStamp());
            reSignWxPayData.SetValue("package", "Sign=WXPay");
            reSignWxPayData.SetValue("sign", reSignWxPayData.MakeSign(weiXinPayConfig.KEY));
            return reSignWxPayData.ToJson();
        }

        public string H5Pay(string subject, string body, string notifyUrl, string returnUrl, PayLog payLog, Payment payment,
            Dictionary<string, string> param = null)
        {
            var config = _configService.Get<WeiXinConfig>();

            WxPayData data = new WxPayData();
            data.SetValue("body", body);
            data.SetValue("out_trade_no", payLog.TransactionNo);
            data.SetValue("total_fee", Convert.ToInt32(payLog.OrderAmount * 100));
            data.SetValue("time_start", DateTime.Now.ToString("yyyyMMddHHmmss"));
            data.SetValue("time_expire", DateTime.Now.AddMinutes(10).ToString("yyyyMMddHHmmss"));
            data.SetValue("goods_tag", body);
            data.SetValue("trade_type", "WAP");
            data.SetValue("nonce_str", WxPayApi.GenerateNonceStr());
            data.SetValue("notify_url", notifyUrl);

            WeiXinPayConfig weiXinPayConfig = new WeiXinPayConfig() { APPID = config.AppId, MCHID = config.MchId, KEY = config.Key };
            WxPayData result = WxPayApi.UnifiedOrder(data, 6, weiXinPayConfig);

            if (result.GetValue("return_code").ToString().Equals("fail", StringComparison.OrdinalIgnoreCase))
                return result.GetValue("return_msg").ToString();

            WxPayData reSignWxPayData = new WxPayData();
            reSignWxPayData.SetValue("appid", result.GetValue("appid"));
            reSignWxPayData.SetValue("partnerid", result.GetValue("mch_id"));
            reSignWxPayData.SetValue("prepayid", result.GetValue("prepay_id"));
            reSignWxPayData.SetValue("noncestr", result.GetValue("nonce_str"));
            reSignWxPayData.SetValue("timestamp", WxPayApi.GenerateTimeStamp());
            reSignWxPayData.SetValue("package", "WAP");
            reSignWxPayData.SetValue("sign", reSignWxPayData.MakeSign(weiXinPayConfig.KEY));

            var deepLink = $"weixin://wap/pay?{HttpUtility.UrlEncode(reSignWxPayData.ToUrl())}";

            return $"<script>location.href='{deepLink}';</script>";
        }
        

        public string WebPay(string subject, string body, string notifyUrl, string returnUrl, PayLog payLog, Payment payment,
            Dictionary<string, string> param = null)
        {
            //统一下单
            var config = _configService.Get<WeiXinConfig>();

            WxRequestHandler packageReqHandler = new WxRequestHandler(null);
            var nonceStr = TenPayV3Util.GetNoncestr();
            //商品Id，用户自行定义
            string productId = DateTime.Now.ToString("yyyyMMddHHmmss");
            //创建请求统一订单接口参数
            packageReqHandler.SetParameter("appid", config.AppId);
            packageReqHandler.SetParameter("mch_id", config.MchId);
            packageReqHandler.SetParameter("nonce_str", nonceStr);
            packageReqHandler.SetParameter("body", body);
            packageReqHandler.SetParameter("out_trade_no", payLog.TransactionNo);
            packageReqHandler.SetParameter("total_fee", Convert.ToInt32(payLog.OrderAmount * 100).ToString());
            packageReqHandler.SetParameter("spbill_create_ip", "127.0.0.1");
            packageReqHandler.SetParameter("notify_url", notifyUrl);
            packageReqHandler.SetParameter("trade_type", TenPayV3Type.NATIVE.ToString());
            packageReqHandler.SetParameter("product_id", productId);
            string sign = packageReqHandler.CreateMd5Sign("key", config.Key);
            packageReqHandler.SetParameter("sign", sign);
            string data = packageReqHandler.ParseXML();
            //调用统一订单接口
#pragma warning disable 618
            var result = TenPayV3.Unifiedorder(data);
#pragma warning restore 618
            var unifiedorderRes = XDocument.Parse(result);
            string codeUrl = unifiedorderRes.Element("xml")?.Element("code_url")?.Value;
            return codeUrl;
        }

        [HttpPost]
        public WxPayData GetNotifyData(HttpRequestBase request, string key)
        {
            //接收从微信后台POST过来的数据
            System.IO.Stream s = request.InputStream;
            int count = 0;
            byte[] buffer = new byte[1024];
            StringBuilder builder = new StringBuilder();
            while ((count = s.Read(buffer, 0, 1024)) > 0)
            {
                builder.Append(Encoding.UTF8.GetString(buffer, 0, count));
            }
            s.Flush();
            s.Close();
            s.Dispose();

            Log.Info(this.GetType().ToString(), "Receive data from WeChat : " + builder.ToString());

            //转换数据格式并验证签名
            WxPayData data = new WxPayData();
            try
            {
                data.FromXml(builder.ToString(), key);
                Logger.Warning("微信支付异步通知数据：");
                Logger.Warning(builder.ToString());
            }
            catch (WxPayException ex)
            {
                //若签名错误，则立即返回结果给微信支付后台
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "FAIL");
                res.SetValue("return_msg", ex.Message);
                Log.Error(this.GetType().ToString(), "Sign check error : " + res.ToXml());
            }

            Log.Info(this.GetType().ToString(), "Check sign success");
            return data;
        }

        private bool QueryOrder(string transactionId)
        {
            var config = _configService.Get<WeiXinConfig>();

            WeiXinPayConfig weiXinPayConfig = new WeiXinPayConfig() { APPID = config.AppId, MCHID = config.MchId, KEY = config.Key };

            WxPayData req = new WxPayData();
            req.SetValue("transaction_id", transactionId);
            WxPayData res = WxPayApi.OrderQuery(weiXinPayConfig, req);
            if (res.GetValue("return_code").ToString() == "SUCCESS" &&
                res.GetValue("result_code").ToString() == "SUCCESS")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}