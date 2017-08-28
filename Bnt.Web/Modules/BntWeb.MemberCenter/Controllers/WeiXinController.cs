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
using BntWeb.Services;
using BntWeb.Utility.Extensions;
using BntWeb.Wallet.Services;
using BntWeb.WebApi.Models;
using Senparc.Weixin;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;

namespace BntWeb.MemberCenter.Controllers
{
    public class WeiXinController : Controller
    {
        private readonly IMemberService _memberService;
        private readonly ICurrencyService _currencyService;
        private readonly IWalletService _walletService;
        private readonly UrlHelper _urlHelper;
        private readonly IConfigService _configService;

        public WeiXinController(IMemberService memberService, ICurrencyService currencyService, IWalletService walletService, UrlHelper urlHelper, IConfigService configService)
        {
            _memberService = memberService;
            _currencyService = currencyService;
            _walletService = walletService;
            _urlHelper = urlHelper;
            _configService = configService;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public string AppId => _configService.Get<WeiXinConfig>().AppId;

        public string AppSecret => _configService.Get<WeiXinConfig>().AppSecret;

        public string MchId => _configService.Get<WeiXinConfig>().MchId;

        public string Key => _configService.Get<WeiXinConfig>().Key;

        #region 登陆注册


        /// <summary>
        /// 获取用户的OpenId
        /// </summary>
        /// <returns></returns>
        public ActionResult LoginOAuth(string backUrl = "")
        {
            var routeParas = new RouteValueDictionary{
                    { "area", MemberCenterModule.Area},
                    { "controller", "WeiXin"},
                    { "action", "LoginRedirt"}
                };
            var returnUrl = HostConstObject.HostUrl + _urlHelper.RouteUrl(routeParas);

            var state = string.Empty;
            if (!string.IsNullOrWhiteSpace(backUrl))
                state = Convert.ToBase64String(Encoding.UTF8.GetBytes(backUrl));

            var url = OAuthApi.GetAuthorizeUrl(AppId, returnUrl, state, OAuthScope.snsapi_userinfo);

            return Redirect(url);
        }


        public ActionResult LoginRedirt(string code, string state)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                {
                    return Content("您拒绝了授权！");
                }


                //通过，用code换取access_token
                var openIdResult = OAuthApi.GetAccessToken(AppId, AppSecret, code);
                if (openIdResult.errcode != ReturnCode.请求成功)
                {
                    return Content("错误：" + openIdResult.errmsg);
                }

                var openId = openIdResult.openid;
                //判断是否已经存在
                var oauth = _currencyService.GetSingleByConditon<UserOAuth>(
                    o => o.OAuthType == OAuthType.WeiXin && o.OAuthId.Equals(openId, StringComparison.OrdinalIgnoreCase));
                if (oauth == null)
                {
                    return Redirect("/Html/Member/Register.html?openId=" + openId);
                }

                var user = _memberService.FindMemberById(oauth.MemberId);
                if (user == null)
                    return Redirect("/Html/Member/Register.html?openId=" + openId);
                if (user.LockoutEnabled)
                {
                    return Content("您的账号已经被禁用！");
                }

                var userInfo = HttpUtility.UrlEncode(Convert.ToBase64String(Encoding.UTF8.GetBytes(user.Simplified().ToJson())));
                Logger.Debug(userInfo);
                if (Response != null)
                {
                    HttpCookie aCookie = new HttpCookie("userInfo")
                    {
                        Value = userInfo,
                        Expires = DateTime.Now.AddHours(1)
                    };
                    Response.Cookies.Add(aCookie);
                }
                //跳转到自定义页面
                string backUrl = string.Empty;
                if (!string.IsNullOrWhiteSpace(state))
                    backUrl = Encoding.UTF8.GetString(Convert.FromBase64String(state));
                if (!string.IsNullOrWhiteSpace(backUrl))
                    return Redirect($"{backUrl}");


                return Redirect("/Html/Member/PersonalCenter.html");
            }
            catch (Exception)
            {
                return Redirect("/html/index.html");
            }
        }

        #endregion
    }
}