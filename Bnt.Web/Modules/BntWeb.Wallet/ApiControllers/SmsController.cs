using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BntWeb.Services;
using BntWeb.Validation;
using BntWeb.Wallet.ApiModels;
using BntWeb.WebApi.Filters;
using BntWeb.WebApi.Models;

namespace BntWeb.Wallet.ApiControllers
{
    public class SmsController : BaseApiController
    {
        private readonly ISmsService _smsService;

        public SmsController(ISmsService smsService)
        {
            _smsService = smsService;
        }

        /// <summary>
        /// 发送手机验证码
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [BasicAuthentication]
        public ApiResult SendCode()
        {
            var result = new ApiResult();
            var smsContent = _smsService.SendCode(AuthorizedUser.PhoneNumber, WalletModule.Instance, SmsRequestType.Withdrawals.ToString());
            if (string.IsNullOrWhiteSpace(smsContent.ErrorMessage))
            {
                var data = new
                {
                    smsContent.Key,
                    Code = smsContent.KeyValues["Code"]
                };
                result.SetData(data, true);
            }
            else
            {
                result.msg = smsContent.ErrorMessage;
            }
            return result;
        }
    }
}
