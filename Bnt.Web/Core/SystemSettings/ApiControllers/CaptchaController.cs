using System.Web.Http;
using BntWeb.Core.SystemSettings.Services;
using BntWeb.WebApi.Models;

namespace BntWeb.Core.SystemSettings.ApiControllers
{
    public class CaptchaController : BaseApiController
    {
        private readonly IDefaultCaptchaService _captchaService;

        public CaptchaController(IDefaultCaptchaService captchaService)
        {
            _captchaService = captchaService;
        }

        /// <summary>
        /// 发送手机图形验证码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResult SendCaptcha()
        {
            var result = new ApiResult();

            var captchaContent=_captchaService.Build();

            var data = new
            {
                captchaContent.Key,
                Code = captchaContent.KeyValues["Code"],
                Image= captchaContent.Image
            };
            result.SetData(data);
            return result;
        }
    }
}