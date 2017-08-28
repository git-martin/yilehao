
using System;
using BntWeb.Caching;
using BntWeb.Core.SystemSettings.Models;
using BntWeb.Data;
using BntWeb.Logging;
using BntWeb.Services;
using Image = System.Drawing.Image;

namespace BntWeb.Core.SystemSettings.Services
{
    public class DefaultCaptchaService : IDefaultCaptchaService
    {
        private readonly ICacheManager _cacheManager;
        private readonly IClock _clock;
        private readonly ISignals _signals;

        public DefaultCaptchaService(IClock clock, ISignals signals, ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
            _clock = clock;
            _signals = signals;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public bool Debug { get; set; }

        public string HostUrl { get; set; }

        public string Account { get; set; }

        public string Password { get; set; }

        public int Interval { get; set; } = 1;

        public bool CaptchaVerifyCodeWithKey(string key, string code, bool needClear = true)
        {
            var captchaContent = _cacheManager.Get(key, ctx =>
            {
                ctx.Monitor(_clock.When(TimeSpan.FromSeconds(2)));
                return CaptchaContent.Empty;
            });

            var success = captchaContent != null && captchaContent.KeyValues.ContainsKey("Code") &&
                   captchaContent.KeyValues["Code"].ToString().Equals(code, StringComparison.OrdinalIgnoreCase);
            if (success && needClear)
            {
                //图形验证码验证成功使用，更新缓存
                _signals.Trigger($"{key}-changed");
            }

            return success;
        }

        public void Clear(string key)
        {
            //图形验证码验证成功使用，更新缓存
            _signals.Trigger($"{key}-changed");
        }

        public CaptchaContent Build()
        {
            bool isSuccess = false;

            VerifyCode codeObj = new VerifyCode();
            codeObj.FontSize = 20;
            codeObj.FWidth = 20;
            string code = codeObj.CreateVerifyCode(4);

            CaptchaContent captchaContent = CaptchaContent.Create(KeyGenerator.GetGuidKey().ToString(), code);

            return _cacheManager.Get(captchaContent.Key, ctx =>
            {
                ctx.Monitor(_clock.When(TimeSpan.FromMinutes(Interval)));
                ctx.Monitor(_signals.When($"{ctx.Key}-changed"));

                try
                {

                    Image img = codeObj.CreateImageCode(code);
                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    //将图像保存到指定的流
                    img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

                    captchaContent.Image = "data:image/jpeg;base64," + Convert.ToBase64String(ms.GetBuffer());
                    isSuccess = true;

                }
                catch (Exception ex)
                {
                    Logger.Error(ex, $"发送图形验证码出错,Key{captchaContent.Key},内容{ex.Message}");
                }

                if (!isSuccess)
                {
                    //短信发送失败了，更新缓存
                    _signals.Trigger($"{ctx.Key}-changed");
                }

                return captchaContent;
            });
        }


    }
}