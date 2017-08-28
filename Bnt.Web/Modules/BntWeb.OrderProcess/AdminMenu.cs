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
using BntWeb.Feedback;
using BntWeb.UI.Navigation;

namespace BntWeb.OrderProcess
{
    public class AdminMenu : INavigationProvider
    {
        //public Localizer T { get; set; }
        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add(OrderProcessModule.Key, OrderProcessModule.DisplayName, OrderProcessModule.Position.ToString(), BuildMenu, new List<string> { "icon-shopping-cart" });
        }

        private void BuildMenu(NavigationItemBuilder menu)
        {
            menu.Add(OrderProcessModule.Key + "-OrdersList", "订单列表", "10",
                item => item
                    .Action("List", "Admin", new { area = OrderProcessModule.Area })
                    .Permission(Permissions.ViewOrder)
                );
            menu.Add(OrderProcessModule.Key + "-RefundList", "订单退款", "20",
                item => item
                    .Action("List", "Refund", new { area = OrderProcessModule.Area })
                    .Permission(Permissions.ViewOrder)
                );
            menu.Add(OrderProcessModule.Key + "-ComplaintList", "订单投诉", "30",
                item => item
                    .Action("List", "Admin", new { area = FeedbackModule.Area, feedbackType = 3, sourceType = "Order" })
                    .Permission(Permissions.ManageComplaint)
                );
            menu.Add(OrderProcessModule.Key + "-ReminderList", "催发货列表", "40",
               item => item
                   .Action("List", "DeliveryReminder", new { area = OrderProcessModule.Area })
                   .Permission(Permissions.ManageOrder)
               );

        }
    }
}