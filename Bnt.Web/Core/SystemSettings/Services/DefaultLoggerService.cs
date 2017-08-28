/* 
    ======================================================================== 
        File name：        DefaultLoggerService
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/5/27 9:41:45
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BntWeb.Core.SystemSettings.Models;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.Environment;
using BntWeb.Logging;
using BntWeb.Security;
using BntWeb.Security.Identity;

namespace BntWeb.Core.SystemSettings.Services
{
    public class DefaultLoggerService : ILoggerService
    {
        private readonly ICurrencyService _currencyService;

        private readonly IUserContainer _userContainer;

        public DefaultLoggerService(ICurrencyService currencyService, IUserContainer userContainer)
        {
            _currencyService = currencyService;
            _userContainer = userContainer;
        }

        public bool Create(string message, IBntWebModule module, SecurityLevel securityLevel = SecurityLevel.Normal, object extension = null)
        {
            var userId = "0";
            var userName = "未知用户";
            try
            {
                var currentUser = _userContainer.CurrentUser;
                if (currentUser != null)
                {
                    userId = currentUser.Id;
                    userName = currentUser.UserName;
                }
            }
            catch (Exception)
            {

            }

            return _currencyService.Create(new SystemLog
            {
                Id = KeyGenerator.GetGuidKey(),
                ModuleKey = module.InnerKey,
                ModuleName = module.InnerDisplayName,
                UserId = userId,
                UserName = userName,
                CreateTime = DateTime.Now,
                SecurityLevel = securityLevel,
                Message = message
            });
        }
    }
}