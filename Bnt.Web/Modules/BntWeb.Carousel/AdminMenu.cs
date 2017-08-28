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

namespace BntWeb.Carousel
{
    public class AdminMenu : INavigationProvider
    {
        //public Localizer T { get; set; }
        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add(CarouselModule.Key, CarouselModule.DisplayName, CarouselModule.Position.ToString(), BuildMenu, new List<string> { "icon-magic" });
        }

        private void BuildMenu(NavigationItemBuilder menu)
        {
            menu.Add(CarouselModule.Key + "-CarouselsList", "轮播列表", "20",
                item => item
                    .Action("GroupList", "Admin", new { area = CarouselModule.Area })
                    .Permission(Permissions.ViewCarousel)
                );

        }

    }
}