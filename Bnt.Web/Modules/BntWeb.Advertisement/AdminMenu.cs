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

namespace BntWeb.Advertisement
{
    public class AdminMenu : INavigationProvider
    {
        //public Localizer T { get; set; }
        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add(AdvertisementModule.Key, AdvertisementModule.DisplayName, AdvertisementModule.Position.ToString(), BuildMenu, new List<string> { "icon-barcode" });
        }

        private void BuildMenu(NavigationItemBuilder menu)
        {
            menu.Add(AdvertisementModule.Key + "-AdvertisementsList", "广告列表", "20",
                item => item
                    .Action("AreaList", "Admin", new { area = AdvertisementModule.Area })
                    .Permission(Permissions.ViewAdvertisement)
                );

        }

    }
}