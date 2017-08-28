using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using BntWeb.MemberBase.Services;
using BntWeb.MemberCenter.ApiModels;
using BntWeb.Security.Identity;
using BntWeb.Services;
using BntWeb.Validation;
using BntWeb.WebApi.Models;

namespace BntWeb.MemberCenter.ApiControllers
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
        public ApiResult SendCode([FromBody]SmsRequestModel request)
        {
            Argument.ThrowIfNullOrEmpty(request.PhoneNumber, "手机号码");

            var result = new ApiResult();
            var type = request.RequestType;
            if (!Enum.IsDefined(typeof(SmsRequestType), type))
                throw new WebApiInnerException("0001", "请求类型参数无效");

            var smsContent = _smsService.SendCode(request.PhoneNumber, MemberCenterModule.Instance, type.ToString());

            if (string.IsNullOrWhiteSpace(smsContent.ErrorMessage))
            {
                var data = new
                {
                    smsContent.Key,
                    Code = smsContent.KeyValues["Code"]
                };

                if (Platform != Platform.Web)
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