/* 
    ======================================================================== 
        File name：		AdminMenu
        Module:			
        Author：		    罗嗣宝
        Create Time：		2016/6/22 10:35:29
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BntWeb.UI.Navigation;

namespace BntWeb.PaymentProcess
{
    public class AdminMenu : INavigationProvider
    {
        //public Localizer T { get; set; }
        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add(PaymentProcessModule.Key, PaymentProcessModule.DisplayName, PaymentProcessModule.Position.ToString(), BuildMenu, new List<string> { "icon-reorder" });
        }

        private void BuildMenu(NavigationItemBuilder menu)
        {
            menu.Add(PaymentProcessModule.Key + "-PaymentsList", "支付方式", "10",
                item => item
                    .Action("List", "Admin", new { area = PaymentProcessModule.Area })
                    .Permission(Permissions.ConfigPayment)
                );
        }
    }
}