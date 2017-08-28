/* 
    ======================================================================== 
        File name：		AdminMenu
        Module:			
        Author：		Daniel.Wu（wujb）
        Create Time：		2016/6/22 10:35:29
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using BntWeb.Data.Services;
using BntWeb.UI.Navigation;

namespace BntWeb.SinglePage
{
    public class AdminMenu : INavigationProvider
    {
        private readonly ICurrencyService _currencyService;

        public AdminMenu(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        //public Localizer T { get; set; }
        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add(SinglePageModule.Key, SinglePageModule.DisplayName, SinglePageModule.Position.ToString(), BuildMenu, new List<string> { "icon-food" });
        }

        private void BuildMenu(NavigationItemBuilder menu)
        {
            menu.Add(SinglePageModule.Key + "-SinglePageList", "单页列表", "10",
                item => item
                    .Action("List", "Admin", new { area =SinglePageModule.Area })
                    .Permission(Permissions.ViewPage)
                );

            var pages = _currencyService.GetAll<Models.SinglePage>();
            foreach (var page in pages)
            {
                menu.Add(SinglePageModule.Key + "-"+ page.Key, page.Title, "10",
                    item => item
                        .Action("ProEdit", "Admin", new { area = SinglePageModule.Area, key = page.Key })
                        .Permission(Permissions.EditPage)
                    );

            }

        }
    }
}