/* 
    ======================================================================== 
        File name：        IRoleService
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/5/24 9:51:52
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using BntWeb.Core.SystemUsers.Models;
using BntWeb.Security.Identity;
using BntWeb.Security.Permissions;
using Microsoft.AspNet.Identity;

namespace BntWeb.Core.SystemUsers.Services
{
    public interface IRoleService : IDependency
    {
        /// <summary>
        /// 获取所有后台可见角色
        /// </summary>
        /// <returns></returns>
        List<Role> GetAdminRoles();

        /// <summary>
        /// 根据Id异步获取角色
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        Role FindById(string roleId);

        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="role"></param>
        /// <param name="permissions"></param>
        /// <returns></returns>
        IdentityResult CreateRole(Role role, List<string> permissions);

        /// <summary>
        /// 更新角色
        /// </summary>
        /// <param name="role"></param>
        /// <param name="permissions"></param>
        /// <returns></returns>
        IdentityResult UpdateRole(Role role, List<string> permissions);

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        IdentityResult Delete(Role role);

        /// <summary>
        /// 根据权限配置文档重新组织权限
        /// </summary>
        /// <param name="permissionProviders"></param>
        List<PermissionGroup> MatchPermissions(IEnumerable<IPermissionProvider> permissionProviders);
    }
}