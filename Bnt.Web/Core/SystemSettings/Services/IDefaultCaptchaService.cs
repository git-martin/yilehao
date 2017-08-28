
using BntWeb.Core.SystemSettings.Models;

namespace BntWeb.Core.SystemSettings.Services
{
    public interface IDefaultCaptchaService : IDependency
    {
        /// <summary>
        /// 验证码校验
        /// </summary>
        /// <param name="key"></param>
        /// <param name="code"></param>
        /// <param name="needClear"></param>
        /// <returns></returns>
        bool CaptchaVerifyCodeWithKey(string key, string code, bool needClear = true);

        /// <summary>
        /// 去除缓存 
        /// </summary>
        /// <param name="key"></param>
        void Clear(string key);

        /// <summary>
        /// 创建图形验证码
        /// </summary>
        /// <returns></returns>
        CaptchaContent Build();
    }
}