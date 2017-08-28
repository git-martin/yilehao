using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Transactions;
using System.Web;
using Autofac;
using BntWeb.Config.Models;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.Environment;
using BntWeb.Logging;
using BntWeb.PaymentProcess.Models;
using BntWeb.PaymentProcess.Services;
using BntWeb.Services;
using BntWeb.SystemMessage.Models;
using BntWeb.SystemMessage.Services;
using BntWeb.Utility.Extensions;
using BntWeb.Wallet;
using BntWeb.Wallet.Models;
using BntWeb.Wallet.Services;
using Com.Alipay;
using PaymentType = BntWeb.PaymentProcess.Models.PaymentType;

namespace BntWeb.PaymentProcess.Payments.Alipay
{
    public class AlipayWithdrawal : IWithdrawalDispatcher
    {
        private readonly IPaymentService _paymentService;
        private readonly ICurrencyService _currencyService;
        private readonly IWalletService _walletService;
        private readonly ISystemMessageService _systemMessageService;
        private readonly IConfigService _configService;

        public AlipayWithdrawal(IPaymentService paymentService, ICurrencyService currencyService, IWalletService walletService, ISystemMessageService systemMessageService, IConfigService configService)
        {
            _paymentService = paymentService;
            _currencyService = currencyService;
            _walletService = walletService;
            _systemMessageService = systemMessageService;
            _configService = configService;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public string Transfer(List<CrashApply> applys, Payment payment, string notifyUrl)
        {
            //服务器异步通知页面路径
            var alipayConfig = _configService.Get<AlipayConfig>();
            var config = new AlipayConfig2
            {
                Partner = alipayConfig.Partner,
                SellerId = alipayConfig.SellerId,
                Md5Key = alipayConfig.MD5Key,
                SignType = SignType.MD5
            };

            string detailData = string.Empty;
            decimal amount = 0;

            foreach (var apply in applys)
            {
                if (apply.ApplyState != ApplyState.ApplyPassed)
                {
                    Logger.Operation($"提现转账失败，扣除冻结金额处理失败，TransactionNo：{apply.TransactionNo}，原因：提现转账申请状态不合法", PaymentProcessModule.Instance, Security.SecurityLevel.Danger);
                    continue;
                }
                detailData += $"{apply.TransactionNo}^{apply.Account}^{apply.RealName}^{ apply.Money}^提现|";
                amount += apply.Money;
                apply.ApplyState = ApplyState.Transfering;
                _currencyService.Update(apply);
            }
            detailData = detailData.Trim('|');

            var paras = new Dictionary<string, object>();
            paras.Add("detail_data", detailData);
            paras.Add("batch_fee", amount);
            paras.Add("batch_num", applys.Count);

            //把请求参数打包成数组
            SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();
            sParaTemp.Add("partner", config.Partner);
            sParaTemp.Add("_input_charset", "utf-8");
            sParaTemp.Add("service", "batch_trans_notify");
            sParaTemp.Add("notify_url", notifyUrl);
            sParaTemp.Add("email", config.SellerId);
            sParaTemp.Add("account_name", alipayConfig.AccountName);
            sParaTemp.Add("pay_date", DateTime.Now.ToString("yyyyMMdd"));
            sParaTemp.Add("batch_no", KeyGenerator.GetOrderNumber());
            sParaTemp.Add("batch_fee", paras["batch_fee"].ToString());
            sParaTemp.Add("batch_num", paras["batch_num"].ToString());
            sParaTemp.Add("detail_data", paras["detail_data"].ToString());

            //建立请求
            string sHtmlText = new Submit(config).BuildRequest(sParaTemp, "get", "确认");

            return sHtmlText;
        }

        /// <summary>
        /// 异步返回结果
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string AsyncReturn(HttpRequestBase request)
        {
            Logger.Debug("支付宝批量转账回调");

            var alipayConfig = _configService.Get<AlipayConfig>();
            var config = new AlipayConfig2
            {
                Partner = alipayConfig.Partner,
                SellerId = alipayConfig.SellerId,
                Md5Key = alipayConfig.MD5Key,
                SignType = SignType.MD5
            };

            SortedDictionary<string, string> sPara = GetRequestPost(request);
            Logger.Debug(sPara.ToJson());

            if (sPara.Count > 0)//判断是否有带返回参数
            {
                Notify aliNotify = new Notify(config);
                bool verifyResult = aliNotify.Verify(sPara, request.Form["notify_id"], request.Form["sign"]);

                if (verifyResult)//验证成功
                {
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //请在这里加上商户的业务逻辑程序代码


                    //——请根据您的业务逻辑来编写程序（以下代码仅作参考）——
                    //获取支付宝的通知返回参数，可参考技术文档中服务器异步通知参数列表

                    //批量付款数据中转账成功的详细信息

                    string successDetails = request.Form["success_details"];
                    Logger.Debug($"支付宝批量转账回调成功数据{successDetails}");
                    if (!string.IsNullOrEmpty(successDetails))
                    {
                        var detaildata = successDetails.Split('|');
                        if (detaildata.Length > 0)
                        {
                            foreach (var detail in detaildata)
                            {
                                using (TransactionScope scope = new TransactionScope())
                                {
                                    //其他操作
                                    var crash = detail.Split('^').FirstOrDefault();
                                    CrashApply crashApply =
                                        _currencyService.GetSingleByConditon<CrashApply>(c => c.TransactionNo == crash);
                                    if (crashApply != null)
                                    {
                                        crashApply.ApplyState = ApplyState.Transferred;
                                        crashApply.TransferTime = DateTime.Now;
                                        crashApply.Description = "支付宝提现成功";
                                        _currencyService.Update(crashApply);
                                        string error;
                                        if (!_walletService.Draw(crashApply.MemberId, WalletType.Cash, crashApply.Money, "提现支出", out error, null, null, true))
                                        {
                                            Logger.Operation($"提现转账成功，扣除冻结金额处理失败，TransactionNo：{crashApply.TransactionNo}，原因：{error}", PaymentProcessModule.Instance, Security.SecurityLevel.Danger);
                                        }

                                        _systemMessageService.CreatePushSystemMessage("提现打款", "您申请的提现已打款，请注意查收", "您申请的提现已打款，请注意查收", crashApply.MemberId, null, null, "CrashApply", WalletModule.Key, MessageCategory.Personal);
                                        //提交
                                        scope.Complete();
                                    }
                                }
                            }
                        }
                    }
                    //批量付款数据中转账失败的详细信息
                    string failDetails = request.Form["fail_details"];
                    Logger.Debug($"支付宝批量转账回调失败数据{failDetails}");
                    if (!string.IsNullOrEmpty(failDetails))
                    {
                        var faildata = failDetails.Split('|');
                        if (faildata.Length > 0)
                        {
                            foreach (var detail in faildata)
                            {
                                using (TransactionScope scope = new TransactionScope())
                                {
                                    var crash = detail.Split('^').FirstOrDefault();
                                    var description = detail.Split('^');
                                    CrashApply crashApply =
                                        _currencyService.GetSingleByConditon<CrashApply>(c => c.TransactionNo == crash);
                                    if (crashApply != null)
                                    {
                                        crashApply.ApplyState = ApplyState.Failure;
                                        crashApply.Description = description[5];
                                        _currencyService.Update(crashApply);

                                        string error;
                                        if (!_walletService.Thaw(crashApply.MemberId, WalletType.Cash, crashApply.Money, "提现失败，解除冻结", out error))
                                        {
                                            Logger.Operation($"提现转账失败，退还金额处理失败，TransactionNo：{crashApply.TransactionNo}，原因：{error}", PaymentProcessModule.Instance, Security.SecurityLevel.Danger);
                                        }
                                        //提交
                                        scope.Complete();
                                    }
                                }
                            }
                        }
                    }

                    //判断是否在商户网站中已经做过了这次通知返回的处理
                    //如果没有做过处理，那么执行商户的业务程序
                    //如果有做过处理，那么不执行商户的业务程序

                    return "success";  //请不要修改或删除

                    //——请根据您的业务逻辑来编写程序（以上代码仅作参考）——

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                }
                else//验证失败
                {
                    Logger.Information("提现支付宝回调处理失败，数据验证失败");
                    return "fail";
                }
            }
            else
            {
                Logger.Information("无通知参数");
                return "无通知参数";
            }
        }
        /// <summary>
        /// 获取支付宝POST过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        public SortedDictionary<string, string> GetRequestPost(HttpRequestBase request)
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
    }
}