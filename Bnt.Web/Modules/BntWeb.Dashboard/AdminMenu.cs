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
using BntWeb.Security;
using BntWeb.UI.Navigation;

namespace BntWeb.Dashboard
{
    public class AdminMenu : INavigationProvider
    {
        //public Localizer T { get; set; }
        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add(DashboardModule.Key, DashboardModule.DisplayName, DashboardModule.Position.ToString(), BuildMenu, new List<string> { "icon-truck" });
            builder.AddImageSet("dashboard")
                .Add("", "控制台", "-9999",
                    menu => menu.Add("BntWeb-Dashboard", "控制台", "1",
                        item => item
                            .Action("Index", "Admin", new { area = "Dashboard" })
                            //.Permission(StandardPermissions.AccessAdminPanel)
                            ), new List<string> { "icon-dashboard" });
        }

        private void BuildMenu(NavigationItemBuilder menu)
        {

            menu.Add(DashboardModule.Key + "-OrderList", "订单统计报表", "10",
               item => item
                   .Action("List", "Statistics", new { area = DashboardModule.Area })

               );
            menu.Add(DashboardModule.Key + "-MemberList", "会员统计报表", "20",
             item => item
                 .Action("MemberList", "Statistics", new { area = DashboardModule.Area })

             );

            //menu.Add(DashboardModule.Key + "-MemberMonthSaleReport", "会员月销售额统计", "30",
            // item => item
            //     .Action("MemberMonthSaleReport", "Statistics", new { area = DashboardModule.Area })
            // );
        }
    }
}