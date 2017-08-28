/* 
    ======================================================================== 
        File name：        DefaultSmsService
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/6/13 15:25:52
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using BntWeb.Caching;
using BntWeb.Core.SystemSettings.Models;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.Environment;
using BntWeb.Logging;
using BntWeb.Security.Identity;
using BntWeb.Services;
using BntWeb.Utility.Extensions;
using BntWeb.Validation;

namespace BntWeb.Core.SystemSettings.Services
{
    public class DefaultSmsService : ISmsService
    {
        private readonly ICacheManager _cacheManager;
        private readonly IClock _clock;
        private readonly ISignals _signals;
        private readonly ICurrencyService _currencyService;

        public DefaultSmsService(IClock clock, ISignals signals, ICurrencyService currencyService, ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
            _clock = clock;
            _signals = signals;
            _currencyService = currencyService;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public bool Debug { get; set; }

        public string HostUrl { get; set; }

        public string Account { get; set; }

        public string Password { get; set; }

        public int EffectiveTime { get; set; } = 5;

        public int Interval { get; set; } = 1;

        public SmsContent SendCode(string phone, IBntWebModule module = null, string type = null)
        {
            var code = "0000";
            if (!Debug)
            {
                Random rad = new Random();
                int value = rad.Next(1000, 10000);
                code = value.ToString();
            }

            var message = $"您的短信验证码是:{code}";
            var smsContent = SmsContent.Create(phone, message);
            if (module != null)
                smsContent.Module = module;
            if (type != null)
                smsContent.InnerType = type;

            smsContent.KeyValues.Add("Code", code);

            return Send(smsContent);
        }

        public bool VerifyCode(string phone, string code, IBntWebModule module = null, string type = null, bool invalid = true)
        {
            var key = SmsContent.GetKey(phone, module, type);
            return VerifyCodeWithKey(key, code, invalid);
        }

        public bool VerifyCodeWithKey(string key, string code, bool invalid = true)
        {
            var smsContent = _cacheManager.Get(key, ctx =>
            {
                ctx.Monitor(_clock.When(TimeSpan.FromSeconds(2)));
                return SmsContent.Empty;
            });

            var success = smsContent != null && smsContent.KeyValues.ContainsKey("Code") &&
                   smsContent.KeyValues["Code"].ToString().Equals(code, StringComparison.OrdinalIgnoreCase);
            if (success && invalid)
            {
                //短信验证成功使用，更新缓存
                _signals.Trigger($"{key}-changed");
            }

            return success;
        }

        public SmsContent Send(SmsContent smsContent)
        {
            Resend: var sms = _cacheManager.Get(smsContent.Key, ctx =>
             {
                 ctx.Monitor(_clock.When(TimeSpan.FromMinutes(EffectiveTime)));
                 ctx.Monitor(_signals.When($"{ctx.Key}-changed"));

                 try
                 {
                     var smsLog = new SmsLog
                     {
                         Id = KeyGenerator.GetGuidKey(),
                         ModuleKey = smsContent.Module.InnerKey,
                         ModuleName = smsContent.Module.InnerDisplayName,
                         Message = smsContent.Message,
                         Phone = smsContent.Phone,
                         UserId = smsContent.SendToUser?.Id,
                         UserName = smsContent.SendToUser?.UserName,
                         CreateTime = DateTime.Now
                     };

                     smsContent.SendTime = DateTime.Now;
                     if (!Debug)
                     {
                         Logger.Warning("开始发送短信");
                         //发送到短信接口
                         //var url =
                         // $"{HostUrl}/HttpSendSM?account={Account}&pswd={Password}&mobile={smsContent.Phone}&msg={smsContent.Message}&needstatus=true";
                         var sign = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile("10010312" + "sef78f42cv1c124s8z2s", "MD5").ToLower();
                         var content =HttpUtility.UrlEncode(Convert.ToBase64String(Encoding.UTF8.GetBytes(smsContent.Message)));
                         var url = $"http://ysms.game2palm.com:8899/smsAccept/sendSms.action?sid=710312&mobi={smsContent.Phone}&sign={sign}&msg={content}";
                         Logger.Warning("短信链接" + url);
                         Uri uri = new Uri(url);
                         WebRequest webreq = WebRequest.Create(uri);
                         webreq.Method = "post";
                         webreq.ContentType = "application/x-www-form-urlencoded";
                         Stream stream = webreq.GetResponse().GetResponseStream();

                         Logger.Warning("短信结果：" + stream);
                         if (stream != null)
                         {
                             StreamReader streamReader = new StreamReader(stream, Encoding.UTF8);
                             string returnString = streamReader.ReadToEnd();
                             var returnCode = returnString.Split('|')[0];

                             if (returnCode == "1000")
                             {
                                 _currencyService.Create(smsLog);
                             }
                             else
                             {
                                 throw new Exception($"短信发送失败，错误码：{returnCode}");
                             }
                         }
                         else
                         {
                             throw new Exception("短信发送失败，获取接口返回数据失败");
                         }
                     }
                     else
                     {
                         _currencyService.Create(smsLog);
                     }

                     smsContent.SendStatus = SmsSendStatus.Success;
                 }
                 catch (Exception ex)
                 {
                     smsContent.ErrorMessage = ex.Message;

                     Logger.Error(ex, $"发送手机短信出错,手机号{smsContent.Phone},内容{smsContent.Message}");
                 }

                 if (smsContent.SendStatus == SmsSendStatus.Failed)
                 {
                     //短信发送失败了，更新缓存
                     _signals.Trigger($"{ctx.Key}-changed");
                 }

                 return smsContent;
             });

            if (sms.SendTime.AddMinutes(Interval) < DateTime.Now)
            {
                _signals.Trigger($"{sms.Key}-changed");
                goto Resend;
            }

            return sms;
        }

        public int QueryResidualQuantity()
        {
            return _cacheManager.Get("query-residual-quantity", ctx =>
            {
                ctx.Monitor(_clock.When(TimeSpan.FromSeconds(10)));

                //发送到短信接口
                var url = $"{HostUrl}/QueryBalance?account={Account}&pswd={Password}";
                Uri uri = new Uri(url);
                WebRequest webreq = WebRequest.Create(uri);
                webreq.Method = "get";
                Stream stream = webreq.GetResponse().GetResponseStream();
                if (stream != null)
                {
                    StreamReader streamReader = new StreamReader(stream, Encoding.UTF8);
                    string returnString = streamReader.ReadToEnd();
                    var returnData = returnString.Replace("\r\n", ",").Replace('\r', ',').Replace('\n', ',').Split(',');
                    var returnCode = returnData[1];

                    if (returnCode == "0")
                    {
                        return returnData.Last().To<int>();
                    }
                    else
                    {
                        return -1;
                    }
                }
                else
                {
                    return -1;
                }
            });
        }

        public void Invalid(string phone, IBntWebModule module = null, string type = null)
        {
            var key = SmsContent.GetKey(phone, module, type);
            Invalid(key);
        }

        public void Invalid(string key)
        {
            //更新缓存
            _signals.Trigger($"{key}-changed");
        }
    }
}