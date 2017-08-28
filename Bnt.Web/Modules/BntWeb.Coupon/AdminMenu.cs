using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using BntWeb.UI.Navigation;

namespace BntWeb.Coupon
{
    public class AdminMenu : INavigationProvider
    {
        //public Localizer T { get; set; }
        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add(CouponModule.Key, CouponModule.DisplayName, CouponModule.Position.ToString(), BuildMenu, new List<string> { "icon-tags" });
        }

        private void BuildMenu(NavigationItemBuilder menu)
        {
            menu.Add(CouponModule.Key + "-CouponList", "优惠券列表", "10",
                item => item
                    .Action("List", "Admin", new { area = CouponModule.Area })
                    .Permission(Permissions.Coupon)
                );
           
        }
    }
}