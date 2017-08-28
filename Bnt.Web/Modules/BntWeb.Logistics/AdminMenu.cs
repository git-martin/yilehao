using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BntWeb.UI.Navigation;

namespace BntWeb.Logistics
{
    public class AdminMenu : INavigationProvider
    {
        //public Localizer T { get; set; }
        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add(LogisticsModule.Key, LogisticsModule.DisplayName, LogisticsModule.Position.ToString(), BuildMenu, new List<string> { "icon-truck" });
        }

        private void BuildMenu(NavigationItemBuilder menu)
        {
            menu.Add(LogisticsModule.Key + "-ShippingList", "配送方式", "10",
                item => item
                    .Action("List", "Shipping", new { area = LogisticsModule.Area })
                    .Permission(Permissions.ViewShipping)
                );

            menu.Add(LogisticsModule.Key + "-ShippingAreaList", "配送区域", "20",
                item => item
                    .Action("List", "ShippingArea", new { area = LogisticsModule.Area })
                    .Permission(Permissions.ViewShipping)
                );
        }
    }
}