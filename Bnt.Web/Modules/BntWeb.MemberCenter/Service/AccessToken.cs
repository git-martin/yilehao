using System;
using System.IO;
using System.Net;
using System.Text;
using BntWeb.Caching;
using BntWeb.Config.Models;
using BntWeb.Data.Services;
using BntWeb.Logging;
using BntWeb.PaymentProcess.Models;
using BntWeb.PaymentProcess.Services;
using BntWeb.Services;
using BntWeb.Utility.Extensions;
using BntWeb.WebApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BntWeb.MemberCenter.Service
{
    public class AccessToken : IAccessToken
    {
        private readonly ICacheManager _cacheManager;
        private readonly IPaymentService _paymentService;
        private readonly IClock _clock;
        private readonly ISignals _signals;
        private readonly ICurrencyService _currencyService;
        private readonly IConfigService _configService;

        public AccessToken(IClock clock, ISignals signals, ICurrencyService currencyService, IPaymentService paymentService, ICacheManager cacheManager, IConfigService configService)
        {
            _cacheManager = cacheManager;
            _configService = configService;
            _paymentService = paymentService;
            _cacheManager = cacheManager;
            _clock = clock;
            _signals = signals;
            _currencyService = currencyService;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }
        private WeiXinConfig WeiXinConfig()
        {
            var payment = _paymentService.LoadPayment(PaymentType.WeiXin.ToString());

            if (payment == null || !payment.Enabled)
                throw new WebApiInnerException("0004", "支付方式不合法或已停用");
            var config = _configService.Get<WeiXinConfig>();
            if (config == null)
                throw new WebApiInnerException("0005", "支付方式不合法");
            return config;
        }

        public string AppId
        {
            get
            {
                var param = WeiXinConfig();
                return param.AppId;
            }
        }

        public string AppSecret
        {
            get
            {
                var param = WeiXinConfig();
                return param.AppSecret;
            }
        }

        public string MchId
        {
            get
            {
                var param = WeiXinConfig();
                return param.MchId;
            }
        }

        public string Key
        {
            get
            {
                var param = WeiXinConfig();
                return param.Key;
            }
        }



        public string GetAppId()
        {

            return AppId;

        }



        public string GetToken()
        {
            //return _cacheManager.Get("AccessToken", ctx =>
            //{
            // 也可以这样写:

            string res = "";
            var req =
                (HttpWebRequest)
                    HttpWebRequest.Create(
                        "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + AppId +
                        "&secret=" + AppSecret + "");

            req.Method = "GET";
            using (WebResponse wr = req.GetResponse())
            {
                HttpWebResponse myResponse = (HttpWebResponse)req.GetResponse();


                StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);


                string content = reader.ReadToEnd();

                JObject jd = content.DeserializeJsonToObject<JObject>();
                res = (string)jd["access_token"];
            }

            //ctx.Monitor(_signals.When("AccessToken-changed"));
            return res;

            //});
        }

        /// <summary>
        /// 获取jsapi_ticket
        /// 有效期7200秒，开发者必须在自己的服务全局缓存jsapi_ticket
        /// </summary>
        /// <returns></returns>
        public TicketModel GetJsApiTicket(DateTime dtNow)
        {
            //return _cacheManager.Get("JsApiTicket", ctx =>
            //{
            var accessToken = GetToken();

            var url = "https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token=" + accessToken +
                      "&type=jsapi";
            var resStr = HttpGet(url);
            var model = JsonConvert.DeserializeObject<TicketModel>(resStr);
            //请求成功了
            var dt = DateTime.Now.AddSeconds(Convert.ToInt32(model.expires_in));

            //ctx.Monitor(_clock.When(TimeSpan.FromMilliseconds(ConvertDateTimeInt(dt) - ConvertDateTimeInt(dtNow))));
            //ctx.Monitor(_signals.When("JsApiTicket-changed"));

            return model;

            //});
        }



        /// <summary>  
        /// DateTime时间格式转换为Unix时间戳格式  
        /// </summary>  
        ///<param name="time"> DateTime时间格式  
        /// <returns>Unix时间戳格式</returns>  
        public int ConvertDateTimeInt(DateTime time)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }




        /// <summary>
        /// HttpGet请求
        /// </summary>
        ///<param name="url">
        /// <returns></returns>
        private static string HttpGet(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            return retString;
        }
    }

    public class AccessTokenModel
    {
        public string access_token;

        public string expires_in;
    }

    public class TicketModel
    {
        public string errcode;

        public string errmsg;

        public string ticket;

        public string expires_in;
    }
}