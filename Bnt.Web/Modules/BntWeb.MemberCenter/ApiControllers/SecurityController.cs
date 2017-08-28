using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using BntWeb.Caching;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.MemberBase.Models;
using BntWeb.MemberBase.Services;
using BntWeb.MemberCenter.ApiModels;
using BntWeb.MemberCenter.Service;
using BntWeb.Security;
using BntWeb.Security.Identity;
using BntWeb.Services;
using BntWeb.Validation;
using BntWeb.WebApi.Filters;
using BntWeb.WebApi.Models;
using Microsoft.AspNet.Identity;

namespace BntWeb.MemberCenter.ApiControllers
{
    public class SecurityController : BaseApiController
    {
        private readonly IMemberService _memberService;
        private readonly DefaultUserManager _userManager;
        private readonly IUserContainer _userContainer;
        private readonly ISecurityService _securityService;
        private readonly ICurrencyService _currencyService;
        private readonly ISmsService _smsService;
        private readonly ISignals _signals;
        private readonly IAccessToken _accessToken;

        public SecurityController(IMemberService memberService, DefaultUserManager userManager, IUserContainer userContainer, ISecurityService securityService, ICurrencyService currencyService, ISmsService smsService, ISignals signals, IAccessToken accessToken)
        {
            _memberService = memberService;
            _userManager = userManager;
            _userContainer = userContainer;
            _securityService = securityService;
            _currencyService = currencyService;
            _smsService = smsService;
            _signals = signals;
            _accessToken = accessToken;
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResult Login([FromBody]MemberLoginModel member)
        {
            Argument.ThrowIfNullOrEmpty(member.PhoneNumber, "手机号码");
            Argument.ThrowIfNullOrEmpty(member.Password, "登陆密码");

            var user = Platform == Platform.Web? _userManager.Find(member.PhoneNumber, member.Password): _userManager.Find(member.PhoneNumber, _securityService.Decrypt(member.Password));
            var result = Login(member.MobileDevice, user, member.OpenId);

            return result;
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResult LoginWithSms([FromBody]MemberLoginWithSmsModel member)
        {
            Argument.ThrowIfNullOrEmpty(member.PhoneNumber, "手机号码");
            Argument.ThrowIfNullOrEmpty(member.SmsVerifyCode, "短信验证码");

            if (!_smsService.VerifyCode(member.PhoneNumber, member.SmsVerifyCode, MemberCenterModule.Instance, SmsRequestType.Login.ToString()))
                throw new WebApiInnerException("0001", "短信验证码验证失败");

            var user = _userManager.FindByName(member.PhoneNumber);
            var result = Login(member.MobileDevice, user, member.OpenId);

            return result;
        }



        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="oauthLoginModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResult LoginWithOAuth([FromBody]OAuthLoginModel oauthLoginModel)
        {
            var result = new ApiResult();
            var oauth = _currencyService.GetSingleByConditon<UserOAuth>(
                o => o.OAuthType == oauthLoginModel.OAuthType && o.OAuthId == oauthLoginModel.OAuthId);
            if (oauth == null)
            {
                result.SetData(null);
            }
            else
            {
                var user = _userManager.FindById(oauth.MemberId);
                result = Login(oauthLoginModel.MobileDevice, user);
            }
            return result;
        }

        private ApiResult Login(string mobileDevice, User user, string openId = "")
        {
            var result = new ApiResult();
            if (user != null && user.UserType == UserType.Member)
            {
                if (user.LockoutEnabled)
                    throw new WebApiInnerException("0002", "此用户已经禁止登录");

                var token = KeyGenerator.GetGuidKey().ToString();
                user.DynamicToken = token;
                user.MobileDevice = mobileDevice;

                var needClearUser = _userManager.Users.Where(u => u.MobileDevice.Equals(mobileDevice, StringComparison.OrdinalIgnoreCase) && !u.Id.Equals(user.Id, StringComparison.OrdinalIgnoreCase)).ToArray();
                foreach (var item in needClearUser)
                {
                    item.MobileDevice = "";
                    _userManager.Update(item);
                }
                _userManager.Update(user);

                //绑定微信
                if (!string.IsNullOrWhiteSpace(openId))
                {
                    _currencyService.DeleteByConditon<UserOAuth>(uo => uo.OAuthType == OAuthType.WeiXin && (uo.OAuthId.Equals(openId, StringComparison.OrdinalIgnoreCase) || uo.MemberId.Equals(user.Id, StringComparison.OrdinalIgnoreCase)));

                    _currencyService.Create(new UserOAuth()
                    {
                        Id = KeyGenerator.GetGuidKey(),
                        MemberId = user.Id,
                        OAuthId = openId,
                        OAuthType = OAuthType.WeiXin
                    });
                }

                _signals.Trigger(_userContainer.UserChangedSignalName);
                _signals.Trigger($"member_{user.Id}_changed");
                result.SetData(_memberService.FindMember(user).Simplified());
            }
            else
            {
                throw new WebApiInnerException("0003", "用户不存在或密码错误");
            }
            return result;
        }

        [HttpPost]
        [BasicAuthentication]
        public ApiResult Logout()
        {
            var result = new ApiResult();
            var user = _userManager.FindById(AuthorizedUser.Id);
            if (user != null && user.UserType == UserType.Member)
            {
                var token = KeyGenerator.GetGuidKey().ToString();
                user.DynamicToken = token;
                user.MobileDevice = "";
                _userManager.Update(user);

                _signals.Trigger(_userContainer.UserChangedSignalName);
                _signals.Trigger($"member_{user.Id}_changed");
            }

            return result;
        }

        [HttpGet]
        public ApiResult GetJsApiConfig(string strUrl)
        {
            string appId = _accessToken.GetAppId();
            DateTime dt = DateTime.Now;
            string nonceStr = Guid.NewGuid().ToString();
            var accessToken = _accessToken.GetToken();
            TicketModel signature = _accessToken.GetJsApiTicket(dt);
            string timestamp = _accessToken.ConvertDateTimeInt(dt).ToString();
            string url = strUrl;
            String[] arr = new String[] { accessToken, timestamp, nonceStr, url };
            // 将token、timestamp、nonce三个参数进行字典序排序 
            Array.Sort<String>(arr);

            StringBuilder content = new StringBuilder();
            for (int i = 0; i < arr.Length; i++)
            {
                content.Append(arr[i]);
            }

            var string1Builder = new StringBuilder();
            string1Builder.Append("jsapi_ticket=").Append(signature.ticket).Append("&")
                          .Append("noncestr=").Append(nonceStr).Append("&")
                          .Append("timestamp=").Append(timestamp).Append("&")
                          .Append("url=").Append(url.IndexOf("#") >= 0 ? url.Substring(0, url.IndexOf("#")) : url);
            var string1 = string1Builder.ToString();
            //return Util.Sha1(string1);
            var tmpStr1 = FormsAuthentication.HashPasswordForStoringInConfigFile(string1, "SHA1");


            //String tmpStr = SHA1_Encrypt(content.ToString());
            //加密/校验流程： 
            //1. 将token、timestamp、nonce三个参数进行字典序排序 
            //string[] ArrTmp = { Token, timestamp, nonce };
            //Array.Sort(ArrTmp);//字典排序 
            //                   //2.将三个参数字符串拼接成一个字符串进行sha1加密 
            //string tmpStr = string.Join("", ArrTmp);
            //var tmpStr = FormsAuthentication.HashPasswordForStoringInConfigFile(content.ToString(), "SHA1");
            //tmpStr = tmpStr.ToLower();

            var result = new ApiResult();
            result.SetData(new
            {
                appId = appId,
                timestamp = timestamp,
                nonceStr = nonceStr,
                jsapi_ticket = signature.ticket,
                url = url,
                signature = tmpStr1,
                signature1 = string1,
            });
            return result;

        }
    }
}