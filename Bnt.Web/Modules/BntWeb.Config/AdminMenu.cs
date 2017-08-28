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

using System.Collections.Generic;
using BntWeb.UI.Navigation;

namespace BntWeb.Config
{
    public class AdminMenu : INavigationProvider
    {
        //public Localizer T { get; set; }
        public string MenuName => "admin";

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add(ConfigModule.Key, ConfigModule.DisplayName, ConfigModule.Position.ToString(), BuildMenu, new List<string> { "icon-wrench" });
        }

        private void BuildMenu(NavigationItemBuilder menu)
        {
            menu.Add(ConfigModule.Key + "-WeiXinConfig", "微信配置", "5",
                item => item
                    .Action("Config", "WeiXinConfig", new { area = ConfigModule.Area })
                    .Permission(Permissions.WeiXinConfig)
                );

            menu.Add(ConfigModule.Key + "-AlipayConfig", "支付宝配置", "10",
                item => item
                    .Action("Config", "AlipayConfig", new { area = ConfigModule.Area })
                    .Permission(Permissions.AlipayConfig)
                );

            menu.Add(ConfigModule.Key + "-SystemConfig", "系统配置", "15",
                item => item
                    .Action("Config", "SystemConfig", new { area = ConfigModule.Area })
                    .Permission(Permissions.SystemConfig)
                );
        }

    }
}