/* 
    ======================================================================== 
        File name：        UserService
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/5/24 9:47:13
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BntWeb.Caching;
using BntWeb.Logging;
using BntWeb.Security;
using BntWeb.Security.Identity;
using Microsoft.AspNet.Identity;

namespace BntWeb.Core.SystemUsers.Services
{
    public class UserService : IUserService
    {
        private readonly DefaultUserManager _userManager;
        private readonly DefaultRoleManager _roleManager;
        private readonly IUserContainer _userContainer;
        private readonly ISignals _signals;

        public UserService(DefaultUserManager userManager, DefaultRoleManager roleManager, IUserContainer userContainer,
            ISignals signals)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userContainer = userContainer;
            _signals = signals;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public User FindUserById(string id)
        {
            var user = _userManager.FindById(id);
            if (user != null && user.AllRoles.Count == 0)
            {
                var roles = _userManager.GetRoles(user.Id);
                user.AllRoles = _roleManager.Roles.Where(r => roles.Contains(r.Name)).ToList();
            }
            return user;
        }

        public User FindUserByName(string name)
        {
            var user = _userManager.FindByName(name);
            if (user != null && user.AllRoles.Count == 0)
            {
                var roles = _userManager.GetRoles(user.Id);
                user.AllRoles = _roleManager.Roles.Where(r => roles.Contains(r.Name)).ToList();
            }
            return user;
        }

        public IdentityResult CreateAdminUser(User user, string password, List<string> roles)
        {
            //不是最佳做法，没有事务回滚
            user.UserType = UserType.Admin;
            var result = _userManager.Create(user, password);

            if (result.Succeeded)
            {
                if (roles == null) roles = new List<string>();

                roles.Add("admin");
                foreach (var roleName in roles)
                {
                    result = _userManager.AddToRole(user.Id, roleName);
                    if (!result.Succeeded)
                        break;
                }

                Logger.Operation($"添加后台用户-{user.UserName}:{user.Id}", SystemUsersModule.Instance, SecurityLevel.Warning);
                //更新缓存
                _signals.Trigger(_userContainer.UserChangedSignalName);
            }

            return result;
        }

        public IdentityResult UpdateAdminUser(User oldUser, string oldPassword, string newPassword, List<string> roles, bool modifyRoles)
        {
            //不是最佳做法，没有事务回滚
            //更新
            oldUser.AllRoles.Clear();

            var result = _userManager.Update(oldUser);
            if (!result.Succeeded)
                return result;

            if (!string.IsNullOrEmpty(oldPassword) && !string.IsNullOrEmpty(newPassword))
            {
                result = _userManager.ChangePassword(oldUser.Id, oldPassword, newPassword);
                if (!result.Succeeded)
                    return result;
            }

            if (modifyRoles)
            {
                var allAdminRoles = _roleManager.Roles.Select(r => r.Name).ToArray();
                foreach (var roleName in allAdminRoles)
                {
                    if (_userManager.IsInRole(oldUser.Id, roleName))
                    {
                        result = _userManager.RemoveFromRole(oldUser.Id, roleName);
                        if (!result.Succeeded)
                            return result;
                    }
                }

                if (roles == null) roles = new List<string>();

                roles.Add("admin");
                foreach (var roleName in roles)
                {
                    if (!_userManager.IsInRole(oldUser.Id, roleName))
                    {
                        result = _userManager.AddToRole(oldUser.Id, roleName);
                        if (!result.Succeeded)
                            return result;
                    }
                }
            }

            Logger.Operation($"修改后台用户-{oldUser.UserName}:{oldUser.Id}", SystemUsersModule.Instance, SecurityLevel.Warning);
            //更新缓存
            _signals.Trigger(_userContainer.UserChangedSignalName);

            return result;
        }

        public IdentityResult SetLockoutEnabled(string userId, bool enabled)
        {
            var result = _userManager.SetLockoutEnabled(userId, enabled);
            if (result.Succeeded)
            {
                var user = _userManager.FindById(userId);
                Logger.Operation($"{(enabled ? "启用" : "禁用")}后台用户-{user.UserName}:{user.Id}", SystemUsersModule.Instance, SecurityLevel.Warning);

                //更新缓存
                _signals.Trigger(_userContainer.UserChangedSignalName);
            }
            return result;
        }

        public IdentityResult Delete(User user)
        {
            var newUser = _userManager.Users.FirstOrDefault(u => u.Id.Equals(user.Id, StringComparison.OrdinalIgnoreCase));
            var result = _userManager.Delete(newUser);
            if (result.Succeeded)
            {
                Logger.Operation($"删除后台用户-{user.UserName}:{user.Id}", SystemUsersModule.Instance, SecurityLevel.Warning);
                //更新缓存
                _signals.Trigger(_userContainer.UserChangedSignalName);
            }
            return result;
        }
    }
}