using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BntWeb.Caching;
using BntWeb.Core.SystemUsers.ViewModels;
using BntWeb.Logging;
using BntWeb.Security;
using BntWeb.Security.Identity;
using Microsoft.AspNet.Identity;

namespace BntWeb.Core.SystemUsers.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserContainer _userContainer;
        private readonly ISignals _signals;
        public UserController(DefaultUserManager userManager, IUserContainer userContainer, ISignals signals)
        {
            UserManager = userManager;
            _signals = signals;
            _userContainer = userContainer;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public UserManager<User> UserManager { get; private set; }

        /// <summary>
        /// 登录页面
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        /// <summary>
        /// 登录操作
        /// </summary>
        /// <param name="model"></param> 
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var signInManager = new DefaultSignInManager(UserManager, Request.GetOwinContext().Authentication);

            // 1. 利用ASP.NET Identity获取用户对象
            var user = await UserManager.FindAsync(model.UserName, model.Password);
            if (user == null || user.UserType != UserType.Admin)
            {
                Logger.Operation($"尝试登录后台失败-{model.UserName}", SystemUsersModule.Instance, SecurityLevel.Warning);
                model.ErrorMessage = "用户名或者密码错误";
                return View(model);
            }

            if (user.LockoutEnabled)
            {
                Logger.Operation($"尝试登录后台失败-{model.UserName}", SystemUsersModule.Instance, SecurityLevel.Warning);
                model.ErrorMessage = "账号已经被禁用";
                return View(model);
            }

            await signInManager.SignInAsync(user, true, model.RememberMe);

            Logger.Operation($"成功登录后台-{user.UserName}:{user.Id}", SystemUsersModule.Instance);
            //更新缓存
            _signals.Trigger(_userContainer.UserChangedSignalName);
            return RedirectToLocal(model.ReturnUrl);
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>

        public ActionResult LogOff()
        {
            HttpContext.GetOwinContext().Authentication.SignOut();
            return RedirectToAction("Login", "User");
        }


        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Admin", new { area = "Dashboard" });
        }
    }
}