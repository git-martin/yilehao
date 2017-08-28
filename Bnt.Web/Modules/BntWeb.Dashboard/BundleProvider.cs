/* 
    ======================================================================== 
        File name：		BundleProvider
        Module:			
        Author：		Daniel.Wu（wujb）
        Create Time：		2016/6/17 15:21:48
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System.Web.Optimization;
using BntWeb.UI.Bundle;

namespace BntWeb.Dashboard
{
    public class BundleProvider : IBundleProvider
    {
        public void RegisterBundles(BundleCollection bundles)
        {
            //Js
            bundles.Add(new ScriptBundle("~/js/plot").Include(
                "~/Resources/Admin/Scripts/flot/jquery.flot.min.js",
                "~/Resources/Admin/Scripts/flot/jquery.flot.pie.min.js",
                "~/Resources/Admin/Scripts/flot/jquery.flot.resize.min.js"));

            bundles.Add(new ScriptBundle("~/js/echarts").Include(
                "~/Resources/Common/Vendors/echarts/echarts.min.js"));

            bundles.Add(new ScriptBundle("~/js/statistical/order").Include(
"~/Modules/BntWeb.Dashboard/Content/Scripts/statistical.order.js"));

            bundles.Add(new ScriptBundle("~/js/statistical/member").Include(
"~/Modules/BntWeb.Dashboard/Content/Scripts/statistical.member.js"));


        }

    }
}