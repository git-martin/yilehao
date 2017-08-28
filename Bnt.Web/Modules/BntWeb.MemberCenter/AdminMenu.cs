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

namespace BntWeb.MemberCenter
{
    public class AdminMenu : INavigationProvider
    {
        //public Localizer T { get; set; }
        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add(MemberCenterModule.Key, MemberCenterModule.DisplayName, MemberCenterModule.Position.ToString(), BuildMenu, new List<string> { "icon-group" });
        }

        private void BuildMenu(NavigationItemBuilder menu)
        {
            menu.Add(MemberCenterModule.Key + "-MembersList", "会员列表", "20",
                item => item
                    .Action("List", "Admin", new { area = MemberCenterModule.Area })
                    .Permission(Permissions.ViewMember)
                );

        }

    }
}