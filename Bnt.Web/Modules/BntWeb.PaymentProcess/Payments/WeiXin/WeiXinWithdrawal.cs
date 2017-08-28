using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Transactions;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using Autofac;
using BntWeb.Config.Models;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.Environment;
using BntWeb.Logging;
using BntWeb.MemberBase.Models;
using BntWeb.Mvc;
using BntWeb.PaymentProcess.Models;
using BntWeb.PaymentProcess.Services;
using BntWeb.Services;
using BntWeb.SystemMessage.Models;
using BntWeb.SystemMessage.Services;
using BntWeb.Utility.Extensions;
using BntWeb.Wallet;
using BntWeb.Wallet.Models;
using BntWeb.Wallet.Services;
using BntWeb.WebApi.Models;
using Com.Alipay;
using Senparc.Weixin.HttpUtility;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.TenPayLibV3;
using Senparc.Weixin.QY.AdvancedAPIs;
using PaymentType = BntWeb.PaymentProcess.Models.PaymentType;

namespace BntWeb.PaymentProcess.Payments.WeiXin
{
    public class WeiXinWithdrawal : IWithdrawalDispatcher
    {
        private readonly ICurrencyService _currencyService;
        private readonly IWalletService _walletService;
        private readonly ISystemMessageService _systemMessageService;
        private readonly IConfigService _configService;

        public WeiXinWithdrawal(ICurrencyService currencyService, IWalletService walletService, ISystemMessageService systemMessageService, IConfigService configService)
        {
            _currencyService = currencyService;
            _walletService = walletService;
            _systemMessageService = systemMessageService;
            _configService = configService;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public string AppId => _configService.Get<WeiXinConfig>().AppId;

        public string AppSecret => _configService.Get<WeiXinConfig>().AppSecret;

        public string MchId => _configService.Get<WeiXinConfig>().MchId;

        public string Key => _configService.Get<WeiXinConfig>().Key;

        public static string RemoteIP
        {
            get
            {
                if (HttpContext.Current == null) return "127.0.0.1";

                var request = HttpContext.Current.Request;

                var result = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(result))
                { result = request.ServerVariables["REMOTE_ADDR"]; }
                if (string.IsNullOrEmpty(result))
                { result = request.Headers["X-Real-IP"]; }
                if (string.IsNullOrEmpty(result))
                { result = request.UserHostAddress; }
                if (string.IsNullOrEmpty(result))
                { result = "127.0.0.1"; }
                return result;
            }
        }

        public string Transfer(List<CrashApply> applys, Payment payment, string notifyUrl)
        {
            int successCount = 0;
            foreach (var crashApply in applys)
            {
                if (crashApply.ApplyState != ApplyState.ApplyPassed)
                {
                    Logger.Operation($"提现转账失败，扣除冻结金额处理失败，TransactionNo：{crashApply.TransactionNo}，原因：提现转账申请状态不合法", PaymentProcessModule.Instance, Security.SecurityLevel.Danger);
                    continue;
                }
                //判断是否已经存在
                var oauth = _currencyService.GetSingleByConditon<UserOAuth>(
                    o => o.OAuthType == OAuthType.WeiXin && o.MemberId.Equals(crashApply.MemberId, StringComparison.OrdinalIgnoreCase));
                if (oauth == null)
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        //其他操作
                        string error;

                        if (_walletService.Thaw(crashApply.MemberId, WalletType.Cash, crashApply.Money, "提现失败", out error))
                        {
                            crashApply.ApplyState = ApplyState.Failure;
                            _currencyService.Update(crashApply);
                            //提交
                            scope.Complete();
                        }
                        else
                        {
                            Logger.Operation($"提现转账失败，扣除冻结金额处理失败，TransactionNo：{crashApply.TransactionNo}，原因：{error}", PaymentProcessModule.Instance, Security.SecurityLevel.Danger);
                        }
                    }
                    continue;
                }

                //创建支付应答对象
                WxRequestHandler packageReqHandler = new WxRequestHandler(HttpContext.Current);
                //初始化
                packageReqHandler.Init();

                var nonceStr = TenPayV3Util.GetNoncestr();
                //设置package订单参数
                packageReqHandler.SetParameter("mch_appid", AppId);                                                     //公众账号ID
                packageReqHandler.SetParameter("mchid", MchId);                                                    //商户号
                packageReqHandler.SetParameter("nonce_str", nonceStr);                                              //随机字符串
                packageReqHandler.SetParameter("partner_trade_no", crashApply.TransactionNo);
                packageReqHandler.SetParameter("openid", oauth.OAuthId);
                packageReqHandler.SetParameter("check_name", "OPTION_CHECK");
                packageReqHandler.SetParameter("re_user_name", crashApply.RealName);
                packageReqHandler.SetParameter("amount", (crashApply.Money * 100).ToString());
                packageReqHandler.SetParameter("desc", "余额提现");
                packageReqHandler.SetParameter("spbill_create_ip", RemoteIP);

                string sign = packageReqHandler.CreateMd5Sign("key", Key);
                packageReqHandler.SetParameter("sign", sign);   //签名

                string data = packageReqHandler.ParseXML();
                Logger.Debug(data);
                var urlFormat = "https://api.mch.weixin.qq.com/mmpaymkttransfers/promotion/transfers";

                var formDataBytes = data == null ? new byte[0] : Encoding.UTF8.GetBytes(data);
                MemoryStream ms = new MemoryStream();
                ms.Write(formDataBytes, 0, formDataBytes.Length);
                ms.Seek(0, SeekOrigin.Begin);//设置指针读取位置
                X509Certificate2 cer = new X509Certificate2($"{AppDomain.CurrentDomain.BaseDirectory}/Config/weixin_apiclient_cert.p12", MchId);

                try
                {
                    var result = RequestUtility.HttpPost(urlFormat, null, ms, timeOut: 10000, cer: cer, checkValidationResult: true);
                    //                    @"<xml>
                    //<return_code><![CDATA[SUCCESS]]></return_code>
                    //<return_msg><![CDATA[]]></return_msg>
                    //<mch_appid><![CDATA[wx8526f7a589c11f7f]]></mch_appid>
                    //<mchid><![CDATA[1393035002]]></mchid>
                    //<device_info><![CDATA[]]></device_info>
                    //<nonce_str><![CDATA[06138BC5AF6023646EDE0E1F7C1EAC75]]></nonce_str>
                    //<result_code><![CDATA[SUCCESS]]></result_code>
                    //<partner_trade_no><![CDATA[2016101312011511532]]></partner_trade_no>
                    //<payment_no><![CDATA[1000018301201610130371564966]]></payment_no>
                    //<payment_time><![CDATA[2016-10-13 19:48:11]]></payment_time>
                    //</xml>"

                    //                    @"<xml>
                    //<return_code><![CDATA[SUCCESS]]></return_code>
                    //<return_msg><![CDATA[付款金额不能小于最低限额.]]></return_msg>
                    //<result_code><![CDATA[FAIL]]></result_code>
                    //<err_code><![CDATA[AMOUNT_LIMIT]]></err_code>
                    //<err_code_des><![CDATA[付款金额不能小于最低限额.]]></err_code_des>
                    //</xml>"
                    var res = XDocument.Parse(result);
                    var resultCodeXml = res.Element("xml")?.Element("result_code");
                    if (resultCodeXml != null && resultCodeXml.Value.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase))
                    {
                        successCount++;
                        //提交成功
                        crashApply.ApplyState = ApplyState.Transferred;
                        crashApply.TransferTime = DateTime.Now;
                        crashApply.Description = "微信提现成功";
                        _currencyService.Update(crashApply);
                        string error;
                        if (
                            !_walletService.Draw(crashApply.MemberId, WalletType.Cash, crashApply.Money, "提现支出",
                                out error, null, null, true))
                        {
                            Logger.Operation($"提现转账成功，扣除冻结金额处理失败，TransactionNo：{crashApply.TransactionNo}，原因：{error}",
                                PaymentProcessModule.Instance, Security.SecurityLevel.Danger);
                        }

                        _systemMessageService.CreatePushSystemMessage("提现打款", "您申请的提现已打款，请注意查收", "您申请的提现已打款，请注意查收",
                            crashApply.MemberId, null, null, "CrashApply", WalletModule.Key, MessageCategory.Personal);
                    }
                    else
                    {
                        var returnMsgXml = res.Element("xml")?.Element("return_msg")?.Value;
                        Logger.Operation($"提现转账失败，TransactionNo：{crashApply.TransactionNo}，原因：{returnMsgXml}", PaymentProcessModule.Instance, Security.SecurityLevel.Danger);
                    }

                    Logger.Debug(result);
                }
                catch (Exception ex)
                {
                    Logger.Debug(ex, "微信企业付款");
                    Logger.Operation($"提现转账失败，TransactionNo：{crashApply.TransactionNo}，未知异常：{ex.Message}", PaymentProcessModule.Instance, Security.SecurityLevel.Danger);

                    continue;
                }
            }

            var resultJson = new DataJsonResult();
            if (successCount < applys.Count)
            {
                resultJson.ErrorMessage = "部分条目处理失败，请查看系统日志";
            }
            resultJson.Success = true; //重置回True
            return resultJson.ToJson();
        }

        /// <summary>
        /// 异步返回结果
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string AsyncReturn(HttpRequestBase request)
        {
            return "";
        }
    }
}