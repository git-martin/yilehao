/* 
    ======================================================================== 
        File name：        AdminMenu
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/5/9 11:41:19
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BntWeb.UI.Navigation;

namespace BntWeb.Core.SystemUsers
{
    public class AdminMenu : INavigationProvider
    {
        //public Localizer T { get; set; }
        public string MenuName => "admin";

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add(SystemUsersModule.Key, SystemUsersModule.DisplayName, SystemUsersModule.Position.ToString(), BuildMenu, new List<string> { "icon-user" });
        }

        private void BuildMenu(NavigationItemBuilder menu)
        {
            menu.Add(SystemUsersModule.Key + "-CreateUser", "新建用户", "10",
                item => item
                    .Action("Edit", "Admin", new { area = SystemUsersModule.Area })
                    .Permission(Permissions.EditUser)
                );

            menu.Add(SystemUsersModule.Key + "-UsersList", "用户列表", "20",
                item => item
                    .Action("List", "Admin", new { area = SystemUsersModule.Area })
                    .Permission(Permissions.ViewUser)
                    .Permission(Permissions.EditUser)
                );

            menu.Add(SystemUsersModule.Key + "-RolesList", "用户角色", "30",
                item => item
                    .Action("List", "Role", new { area = SystemUsersModule.Area })
                    .Permission(Permissions.ViewRole)
                    .Permission(Permissions.EditRole)
                );

        }

    }
}