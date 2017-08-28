using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using BntWeb.Data.Services;
using BntWeb.MemberBase.Models;
using BntWeb.MemberBase.Services;
using BntWeb.Security.Identity;
using BntWeb.Services;
using BntWeb.Utility;
using BntWeb.Utility.Extensions;
using BntWeb.Validation;
using BntWeb.Wallet.ApiModels;
using BntWeb.Wallet.Models;
using BntWeb.Wallet.Services;
using BntWeb.WebApi.Filters;
using BntWeb.WebApi.Models;

namespace BntWeb.Wallet.ApiControllers
{
    public class WalletController : BaseApiController
    {
        private readonly IWalletService _walletService;
        private readonly ISmsService _smsService;
        private readonly ICurrencyService _currencyService;

        public WalletController(IWalletService walletService, ISmsService smsService,ICurrencyService currencyService)
        {
            _walletService = walletService;
            _smsService = smsService;
            _currencyService = currencyService;
        }

        ///// <summary>
        ///// 获取我的钱包
        ///// </summary>
        ///// <param name="walletType"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[BasicAuthentication]
        //public ApiResult GetByType(WalletType walletType = WalletType.Cash)
        //{
        //    var result = new ApiResult();
        //    var wallet = _walletService.GetWalletByMemberId(AuthorizedUser.Id.ToGuid(), walletType);
        //    var withdrawals = 0M;
        //    if (walletType == WalletType.Cash)
        //    {
        //        var memberId = AuthorizedUser.Id.ToGuid();
        //        withdrawals = _currencyService.GetList<CrashApply>(x => x.MemberId == memberId && x.ApplyState == ApplyState.Applying).ToList().Sum(x => x.Money);
        //    }
        //    var data = new
        //    {
        //        Available = wallet.Available,
        //        Withdrawals = withdrawals
        //    };
        //    result.SetData(data);
        //    return result;
        //}

        /// <summary>
        /// 获取我的现金钱包
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [BasicAuthentication]
        public ApiResult GetWalletCash()
        {
            var result = new ApiResult();
            var memberId = AuthorizedUser.Id;
            var wallet = _walletService.GetWalletByMemberId(memberId, WalletType.Cash);

            decimal available = 0;
            if (wallet != null)
            {
                available = wallet.Available;
            }

            var data = new
            {
                Available = available,
                Withdrawals = _currencyService.GetList<CrashApply>(x => x.MemberId == memberId && x.ApplyState == ApplyState.Applying).ToList().Sum(x => x.Money)
            };
            result.SetData(data);
            return result;
        }

        /// <summary>
        /// 获取我的可用积分
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [BasicAuthentication]
        public ApiResult GetWalletIntegral()
        {
            var result = new ApiResult();
            var wallet = _walletService.GetWalletByMemberId(AuthorizedUser.Id, WalletType.Integral);

            decimal available = 0;
            if (wallet != null)
            {
                available = wallet.Available;
            }

            var data = new
            {
                Available = available
            };
            result.SetData(data);
            return result;
        }

        /// <summary>
        /// 申请提现
        /// </summary>
        /// <param name="crashapplyModel"></param>
        /// <returns></returns>
        [HttpPost]
        [BasicAuthentication]
        public ApiResult ApplyCrash([FromBody]CrashapplyModel crashapplyModel)
        {
            Argument.ThrowIfNullOrEmpty(crashapplyModel.PaymentType.ToString(), "提现方式");
            
            //if (crashapplyModel.Money < 500)
            //{
            //    throw new WebApiInnerException("0001", "提现金额不得低于500元");
            //}
            //Argument.ThrowIfNullOrEmpty(crashapplyModel.PayPassword, "支付密码");

            Argument.ThrowIfNullOrEmpty(crashapplyModel.RealName, "真实姓名");

            //支付宝提现 账号不能为空
            if (crashapplyModel.PaymentType == PaymentType.Alipay)
                Argument.ThrowIfNullOrEmpty(crashapplyModel.Account, "提现账号");

            //微信提现 需要绑定微信号
            if (crashapplyModel.PaymentType == PaymentType.WeiXin)
            {
                var oauth = _currencyService.GetSingleByConditon<UserOAuth>(
                     o => o.OAuthType == OAuthType.WeiXin && o.MemberId == AuthorizedUser.Id);
                if (oauth == null)
                {
                    throw new WebApiInnerException("0006", "未绑定微信号");
                }
            }

            Argument.ThrowIfNullOrEmpty(crashapplyModel.SmsVerifyCode, "短信验证码");
            if (!_smsService.VerifyCode(AuthorizedUser.PhoneNumber, crashapplyModel.SmsVerifyCode, WalletModule.Instance, SmsRequestType.Withdrawals.ToString()))
                throw new WebApiInnerException("0002", "短信验证码验证失败");

            var result = new ApiResult();
            if (!_walletService.ApplyCrash(AuthorizedUser.Id, crashapplyModel.Account, crashapplyModel.Money, crashapplyModel.PaymentType, crashapplyModel.RealName))
            {
                throw new WebApiInnerException("0005", "未知异常");
            }
            return result;
        }

        

    }
}