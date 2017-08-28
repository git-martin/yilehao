
using System.Collections.Generic;
using BntWeb.UI.Navigation;

namespace BntWeb.Discovery
{
    public class AdminMenu: INavigationProvider
    {
         
        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add(DiscoveryModule.Key, DiscoveryModule.DisplayName, DiscoveryModule.Position.ToString(), BuildMenu, new List<string> { "icon-book" });
        }

        private void BuildMenu(NavigationItemBuilder menu)
        {
            menu.Add(DiscoveryModule.Key + "-List", "发现列表", "20",
                item => item
                    .Action("List", "Admin", new { area = DiscoveryModule.Area })
                    .Permission(Permissions.ViewDiscovery)
                ); 
        }
    }



}