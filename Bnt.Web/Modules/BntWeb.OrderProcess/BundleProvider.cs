/* 
    ======================================================================== 
        File name：		BundleProvider
        Module:			
        Author：		        罗嗣宝
        Create Time：		2016/6/17 15:21:48
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System.Web.Optimization;
using BntWeb.UI.Bundle;

namespace BntWeb.OrderProcess
{
    public class BundleProvider : IBundleProvider
    {
        public void RegisterBundles(BundleCollection bundles)
        {
            //Css
            bundles.Add(new StyleBundle("~/css/admin/order/ordergoods").Include(
                "~/Modules/BntWeb.OrderProcess/Content/Css/OrderGoods.css"));

            //Js
            bundles.Add(new ScriptBundle("~/js/admin/order/list").Include(
                      "~/Modules/BntWeb.OrderProcess/Content/Scripts/order.list.js"));
            bundles.Add(new ScriptBundle("~/js/admin/order/detail").Include(
                      "~/Modules/BntWeb.OrderProcess/Content/Scripts/order.detail.js"));
            bundles.Add(new ScriptBundle("~/js/admin/order/refund/list").Include(
                      "~/Modules/BntWeb.OrderProcess/Content/Scripts/orderrefund.list.js"));
            bundles.Add(new ScriptBundle("~/js/admin/order/refund/detail").Include(
                      "~/Modules/BntWeb.OrderProcess/Content/Scripts/orderrefund.detail.js"));
            bundles.Add(new ScriptBundle("~/js/admin/order/evaluate/detail").Include(
                     "~/Modules/BntWeb.OrderProcess/Content/Scripts/orderevaluate.detail.js"));
            bundles.Add(new ScriptBundle("~/js/admin/order/reminder/list").Include(
                     "~/Modules/BntWeb.OrderProcess/Content/Scripts/orderreminder.list.js"));
        }
    }
}