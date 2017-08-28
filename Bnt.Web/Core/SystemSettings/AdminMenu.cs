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

namespace BntWeb.Core.SystemSettings
{
    public class AdminMenu : INavigationProvider
    {
        //public Localizer T { get; set; }
        public string MenuName => "admin";

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add(SystemSettingsModule.Key, SystemSettingsModule.DisplayName, SystemSettingsModule.Position.ToString(), BuildMenu, new List<string> { "icon-cogs" });
        }

        private void BuildMenu(NavigationItemBuilder menu)
        {
            menu.Add(SystemSettingsModule.Key + "-SoftReleasesList", "软件版本", "7",
                item => item
                    .Action("List", "SoftRelease", new { area = SystemSettingsModule.Area })
                    .Permission(Permissions.SoftRelease)
                );

            menu.Add(SystemSettingsModule.Key + "-FilesList", "文件资源", "10",
                item => item
                    .Action("Index", "File", new { area = SystemSettingsModule.Area })
                    .Permission(Permissions.ViewFile)
                );

            menu.Add(SystemSettingsModule.Key + "-SmsList", "短信日志", "20",
                item => item
                    .Action("List", "Sms", new { area = SystemSettingsModule.Area })
                    .Permission(Permissions.ViewSms)
                );

            menu.Add(SystemSettingsModule.Key + "-LogsList", "日志管理", "30",
                item => item
                    .Action("List", "Log", new { area = SystemSettingsModule.Area })
                    .Permission(Permissions.ViewLog)
                );

            menu.Add(SystemSettingsModule.Key + "-District", "行政区", "40",
                item => item
                    .Action("Index", "District", new { area = SystemSettingsModule.Area })
                    .Permission(Permissions.ViewLog)
                );

            menu.Add(SystemSettingsModule.Key + "-DbBackup", "数据库备份", "50",
                item => item
                    .Action("List", "DbBackup", new { area = SystemSettingsModule.Area })
                    .Permission(Permissions.DbBackup)
                );
        }

    }
}