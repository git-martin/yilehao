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
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using BntWeb.Config.Models;
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
using Com.Alipay;

namespace BntWeb.PaymentProcess.Payments.Alipay
{
    public class AlipayPayment : IPaymentDispatcher
    {
        private readonly IPaymentService _paymentService;
        private readonly ICurrencyService _currencyService;
        private readonly ISystemMessageService _systemMessageService;
        private readonly IOrderService _orderService;
        private readonly IConfigService _configService;

        public AlipayPayment(IPaymentService paymentService, ICurrencyService currencyService, ISystemMessageService systemMessageService, IOrderService orderService, IConfigService configService)
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
            Logger.Warning("进入支付宝支付成功同步通知");

            var payment = _paymentService.LoadPayment(PaymentType.Alipay.ToString());
            var alipayConfig = _configService.Get<AlipayConfig>();

            var config = new AlipayConfig2
            {
                Partner = alipayConfig.Partner,
                SellerId = alipayConfig.SellerId,
                Md5Key = alipayConfig.MD5Key,
                SignType = SignType.MD5
            };

            SortedDictionary<string, string> sPara = GetRequestPostByKey(request);
            Logger.Warning("支付同步返回签名参数:" + Core.CreateLinkString(sPara));
            if (sPara.Count > 0)//判断是否有带返回参数
            {
                Notify aliNotify = new Notify(config);
                string sign = Core.GetSign(sPara);
                bool verifyResult = aliNotify.GetSignVeryfy(sPara, Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(sign)));

                if (verifyResult)//验证成功
                {
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //请在这里加上商户的业务逻辑程序代码


                    //——请根据您的业务逻辑来编写程序（以下代码仅作参考）——
                    //获取支付宝的通知返回参数，可参考技术文档中页面跳转同步通知参数列表

                    //商户订单号
                    string out_trade_no = request.QueryString["out_trade_no"];

                    //支付宝交易号
                    string trade_no = request.QueryString["trade_no"];

                    //交易状态
                    string trade_status = request.QueryString["trade_status"];

                    Logger.Warning("正在同步操作商户订单号：" + out_trade_no);
                    string returnString = "";
                    if (request.QueryString["trade_status"] == "TRADE_FINISHED" || request.QueryString["trade_status"] == "TRADE_SUCCESS")
                    {
                        //判断该笔订单是否在商户网站中已经做过处理
                        //如果没有做过处理，根据订单号（out_trade_no）在商户网站的订单系统中查到该笔订单的详细，并执行商户的业务程序
                        //如果有做过处理，不执行商户的业务程序

                        PayLog payLog =
        _currencyService.GetSingleByConditon<PayLog>(o => o.TransactionNo == out_trade_no);
                        payLog.PayTime = DateTime.Now;
                        payLog.LogStatus = LogStatus.Paid;
                        if (payLog != null && payLog.LogStatus != LogStatus.Paid)
                        {
                            _currencyService.Update<PayLog>(payLog);
                            Order order = _currencyService.GetSingleByConditon<Order>(o => o.Id == payLog.OrderId);
                            order.PayStatus = PayStatus.Paid;
                            order.PayTime = DateTime.Now;
                            order.PaymentId = payment.Id;
                            order.PaymentName = payment.Name;
                            order.OrderStatus = OrderStatus.WaitingForDelivery;
                            _orderService.ChangeOrderStatus(order.Id, order.OrderStatus, order.PayStatus);
                            _currencyService.Update<Order>(order);

                            OrderGoods orderGoods =
    _currencyService.GetSingleByConditon<OrderGoods>(o => o.OrderId == order.Id);
                            if (orderGoods != null)
                            {
                                var dicParams = new Dictionary<string, string>();
                                dicParams.Add("messagetype", "3");
                            }
                        }
                    }
                    else
                    {
                        returnString = "trade_status=" + request.QueryString["trade_status"];
                    }

                    //打印页面
                    returnString += "验证成功<br />";
                    return returnString;

                    //——请根据您的业务逻辑来编写程序（以上代码仅作参考）——

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                }
                else//验证失败
                {
                    Logger.Warning("验证失败");
                    return "验证失败";
                }
            }
            else
            {
                Logger.Warning("无返回参数");
                return "无返回参数";
            }
        }

        /// <summary>
        /// 获取支付宝GET过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        public SortedDictionary<string, string> GetRequestGet(HttpRequestBase request)
        {
            int i = 0;
            SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();
            NameValueCollection coll;
            //Load Form variables into NameValueCollection variable.
            coll = request.QueryString;

            // Get names of all forms into a string array.
            String[] requestItem = coll.AllKeys;

            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], request.QueryString[requestItem[i]]);
            }

            return sArray;
        }

        public string AsyncReturn(HttpRequestBase request)
        {
            var payment = _paymentService.LoadPayment(PaymentType.Alipay.ToString());
            var alipayConfig = _configService.Get<AlipayConfig>();

            var config = new AlipayConfig2
            {
                Partner = alipayConfig.Partner,
                SellerId = alipayConfig.SellerId,
                Md5Key = alipayConfig.MD5Key,
                SignType = SignType.MD5
            };

            SortedDictionary<string, string> sPara = GetRequestPost(request);
            if (sPara.Count > 0)//判断是否有带返回参数
            {
                var aliNotify = new Notify(config);
                bool verifyResult = aliNotify.Verify(sPara, request.Form["notify_id"], request.Form["sign"]);

                if (verifyResult)//验证成功
                {
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //请在这里加上商户的业务逻辑程序代码


                    //——请根据您的业务逻辑来编写程序（以下代码仅作参考）——
                    //获取支付宝的通知返回参数，可参考技术文档中服务器异步通知参数列表

                    //商户订单号
                    string out_trade_no = request.Form["out_trade_no"];
                    //支付宝交易号
                    string trade_no = request.Form["trade_no"];

                    //交易状态
                    string trade_status = request.Form["trade_status"];

                    Logger.Warning("正在异步操作商户订单号：" + out_trade_no);
                    if (trade_status == "TRADE_FINISHED" || trade_status == "TRADE_SUCCESS")
                    {
                        //判断该笔订单是否在商户网站中已经做过处理
                        //如果没有做过处理，根据订单号（out_trade_no）在商户网站的订单系统中查到该笔订单的详细，并执行商户的业务程序
                        //请务必判断请求时的total_fee、seller_id与通知时获取的total_fee、seller_id为一致的
                        //如果有做过处理，不执行商户的业务程序
                        PayLog payLog =
                            _currencyService.GetSingleByConditon<PayLog>(o => o.TransactionNo == out_trade_no);
                        if (payLog != null && payLog.LogStatus == LogStatus.Unpaid)
                        {
                            payLog.PayTime = DateTime.Now;
                            payLog.LogStatus = LogStatus.Paid;
                            _currencyService.Update<PayLog>(payLog);
                            Order order = _currencyService.GetSingleByConditon<Order>(o => o.Id == payLog.OrderId);
                            order.PayStatus = PayStatus.Paid;
                            order.PayTime = DateTime.Now;
                            order.PaymentId = payment.Id;
                            order.PaymentName = payment.Name;
                            order.OrderStatus = OrderStatus.WaitingForDelivery;
                            _orderService.ChangeOrderStatus(order.Id, order.OrderStatus, order.PayStatus);
                            _currencyService.Update<Order>(order);
                        }
                        //注意：
                        //付款完成后，支付宝系统发送该交易状态通知
                    }
                    else
                    {
                        Logger.Warning("支付宝异步回调操作异常，回调数据：" + trade_status);
                        return "fail";
                    }

                    return "success";  //请不要修改或删除

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                }
                else//验证失败
                {
                    Logger.Warning("fail");
                    return "fail";
                }
            }
            else
            {
                Logger.Warning("无通知参数");
                return "无通知参数";
            }
        }

        /// <summary>
        /// 获取支付宝POST过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        private SortedDictionary<string, string> GetRequestPost(HttpRequestBase request)
        {
            int i = 0;
            SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();
            NameValueCollection coll;
            //Load Form variables into NameValueCollection variable.
            coll = request.Form;

            // Get names of all forms into a string array.
            String[] requestItem = coll.AllKeys;

            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], request.Form[requestItem[i]]);
            }

            return sArray;
        }

        private SortedDictionary<string, string> GetRequestPostByKey(HttpRequestBase request)
        {
            int i = 0;
            SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();
            NameValueCollection coll;
            //Load Form variables into NameValueCollection variable.
            coll = request.Form;

            // Get names of all forms into a string array.

            if (coll["value"] != null)
            {
                string value = coll["value"].ToString();

                String[] requestItem = value.Split('&');
                for (i = 0; i < requestItem.Length; i++)
                {
                    System.Text.RegularExpressions.MatchCollection matchs = new System.Text.RegularExpressions.Regex(@"(\w+)=(""\S+"")").Matches(requestItem[i]);
                    if (matchs.Count > 0)
                    {
                        sArray.Add(matchs[0].Groups[1].ToString(), matchs[0].Groups[2].ToString().Replace("\"", ""));
                    }
                }
            }

            return sArray;
        }

        public string GetSignInfo(string subject, string body, string notifyUrl, PayLog payLog, Payment payment, Dictionary<string, string> param = null)
        {
            if (body.Length > 512)
                body = body.Substring(0, 500) + "...";

            var alipayConfig = _configService.Get<AlipayConfig>();

            var config = new AlipayConfig2
            {
                Partner = alipayConfig.Partner,
                SellerId = alipayConfig.SellerId,
                Md5Key = alipayConfig.MD5Key,
                SignType = SignType.MD5
            };

            var paras = new SortedDictionary<string, string>();
            paras.Add("partner", config.Partner);
            paras.Add("seller_id", config.SellerId);
            paras.Add("out_trade_no", payLog.TransactionNo);
            paras.Add("subject", subject);
            paras.Add("body", body);
            paras.Add("total_fee", payLog.OrderAmount.ToString());
            paras.Add("notify_url", notifyUrl);
            paras.Add("service", "mobile.securitypay.pay");
            paras.Add("payment_type", "1");
            paras.Add("_input_charset", "utf-8");
            paras.Add("it_b_pay", "30m");

            var sPara = new Submit(config).BuildRequestPara(paras);
            return Core.CreateLinkStringUrlencode(sPara, Encoding.UTF8);
        }

        public string H5Pay(string subject, string body, string notifyUrl, string returnUrl, PayLog payLog, Payment payment, Dictionary<string, string> param = null)
        {
            var alipayConfig = _configService.Get<AlipayConfig>();

            var config = new AlipayConfig2
            {
                Partner = alipayConfig.Partner,
                SellerId = alipayConfig.SellerId,
                Md5Key = alipayConfig.MD5Key,
                SignType = SignType.MD5
            };

            ////////////////////////////////////////////////////////////////////////////////////////////////

            if (body.Length > 512)
                body = body.Substring(0, 500) + "...";
            //把请求参数打包成数组
            SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();
            sParaTemp.Add("partner", config.Partner);
            sParaTemp.Add("seller_id", config.SellerId);
            sParaTemp.Add("_input_charset", "utf-8");
            sParaTemp.Add("service", "alipay.wap.create.direct.pay.by.user");
            sParaTemp.Add("payment_type", "1");
            sParaTemp.Add("notify_url", notifyUrl);
            sParaTemp.Add("return_url", returnUrl);
            sParaTemp.Add("out_trade_no", payLog.TransactionNo);
            sParaTemp.Add("subject", subject);
            sParaTemp.Add("total_fee", payLog.OrderAmount.ToString());
            sParaTemp.Add("show_url", "");
            sParaTemp.Add("app_pay", "Y");//启用此参数可唤起钱包APP支付。
            sParaTemp.Add("body", body);
            //其他业务参数根据在线开发文档，添加参数.文档地址:https://doc.open.alipay.com/doc2/detail.htm?spm=a219a.7629140.0.0.2Z6TSk&treeId=60&articleId=103693&docType=1
            //如sParaTemp.Add("参数名","参数值");

            //建立请求
            return new Submit(config).BuildRequest(sParaTemp, "get", "确认");
        }

        public string WebPay(string subject, string body, string notifyUrl, string returnUrl, PayLog payLog, Payment payment,
            Dictionary<string, string> param = null)
        {
            var alipayConfig = _configService.Get<AlipayConfig>();

            var config = new AlipayConfig2
            {
                Partner = alipayConfig.Partner,
                SellerId = alipayConfig.Partner,// web支付中这里是用的partner alipayConfig.SellerId
                Md5Key = alipayConfig.MD5Key,
                SignType = SignType.MD5
            };

            ////////////////////////////////////////////////////////////////////////////////////////////////
            if (body.Length > 512)
                body = body.Substring(0, 500) + "...";
            //把请求参数打包成数组
            SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();
            sParaTemp.Add("service", "create_direct_pay_by_user");
            sParaTemp.Add("partner", config.Partner);
            sParaTemp.Add("seller_id", config.SellerId);
            sParaTemp.Add("_input_charset", "utf-8");
            sParaTemp.Add("payment_type", "1");
            sParaTemp.Add("notify_url", notifyUrl);
            sParaTemp.Add("return_url", returnUrl);
            sParaTemp.Add("out_trade_no", payLog.TransactionNo);
            sParaTemp.Add("subject", subject);
            sParaTemp.Add("total_fee", payLog.OrderAmount.ToString());
            sParaTemp.Add("body", body);
            //其他业务参数根据在线开发文档，添加参数.文档地址:https://doc.open.alipay.com/doc2/detail.htm?spm=a219a.7629140.0.0.2Z6TSk&treeId=60&articleId=103693&docType=1
            //如sParaTemp.Add("参数名","参数值");

            //建立请求
            return new Submit(config).BuildRequest(sParaTemp, "get", "确认");
        }
    }
}