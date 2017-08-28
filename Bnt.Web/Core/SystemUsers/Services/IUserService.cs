/* 
    ======================================================================== 
        File name：        IUserService
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/5/24 9:45:37
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BntWeb.Security.Identity;
using Microsoft.AspNet.Identity;

namespace BntWeb.Core.SystemUsers.Services
{
    public interface IUserService : IDependency
    {
        /// <summary>
        /// 根据Id获取用户完整信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        User FindUserById(string id);

        /// <summary>
        /// 根据Name获取用户完整信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        User FindUserByName(string name);

        /// <summary>
        /// 新建后台用户
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        IdentityResult CreateAdminUser(User user, string password, List<string> roles);

        /// <summary>
        /// 更新后台用户
        /// </summary>
        /// <param name="oldUser"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <param name="roles"></param>
        /// <param name="modifyRoles">是否修改角色</param>
        /// <returns></returns>
        IdentityResult UpdateAdminUser(User oldUser, string oldPassword, string newPassword, List<string> roles, bool modifyRoles = true);

        /// <summary>
        /// 解锁和锁定用户
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        IdentityResult SetLockoutEnabled(string userId, bool enabled);

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        IdentityResult Delete(User user);
    }
}