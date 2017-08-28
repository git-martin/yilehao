/* 
    ======================================================================== 
        File name：        Permissions
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/5/9 15:34:55
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BntWeb.Security.Permissions;

namespace BntWeb.Core.SystemUsers
{
    public class Permissions : IPermissionProvider
    {
        private static readonly string CategoryKey = SystemUsersModule.DisplayName;

        public const string ViewUserKey = "Core-SystemUsers-ViewUser";
        public static readonly Permission ViewUser = new Permission { Description = "查看用户", Name = ViewUserKey, Category = CategoryKey };

        public const string EditUserKey = "Core-SystemUsers-EditUser";
        public static readonly Permission EditUser = new Permission { Description = "编辑用户", Name = EditUserKey, Category = CategoryKey };

        public const string SwitchUserKey = "Core-SystemUsers-SwitchUserKey";
        public static readonly Permission SwitchUser = new Permission { Description = "启用禁用", Name = SwitchUserKey, Category = CategoryKey };

        public const string DeleteUserKey = "Core-SystemUsers-DeleteUser";
        public static readonly Permission DeleteUser = new Permission { Description = "删除用户", Name = DeleteUserKey, Category = CategoryKey };

        public const string ViewRoleKey = "Core-SystemUsers-ViewRole";
        public static readonly Permission ViewRole = new Permission { Description = "查看角色", Name = ViewRoleKey, Category = CategoryKey };

        public const string EditRoleKey = "Core-SystemUsers-EditRole";
        public static readonly Permission EditRole = new Permission { Description = "编辑角色", Name = EditRoleKey, Category = CategoryKey };

        public const string DeleteRoleKey = "Core-SystemUsers-DeleteRole";
        public static readonly Permission DeleteRole = new Permission { Description = "删除角色", Name = DeleteRoleKey, Category = CategoryKey };

        public int Position => SystemUsersModule.Position;

        public string Category => CategoryKey;

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ViewUser,
                EditUser,
                SwitchUser,
                DeleteUser,
                ViewRole,
                EditRole,
                DeleteRole
            };
        }
    }
}