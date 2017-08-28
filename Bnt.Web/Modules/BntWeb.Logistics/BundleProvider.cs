using System.Web.Optimization;
using BntWeb.UI.Bundle;

namespace BntWeb.Logistics
{
    public class BundleProvider : IBundleProvider
    {
        public void RegisterBundles(BundleCollection bundles)
        {
            //Css
            bundles.Add(new StyleBundle("~/js/admin/logistics/style").Include(
                "~/Modules/BntWeb.Logistics/Content/Css/base.css"));

            //Js
            bundles.Add(new ScriptBundle("~/js/admin/logistics/shipping/list").Include(
                      "~/Modules/BntWeb.Logistics/Content/Scripts/shipping.list.js"));

            bundles.Add(new ScriptBundle("~/js/admin/logistics/shippingarea/list").Include(
                      "~/Modules/BntWeb.Logistics/Content/Scripts/shippingarea.list.js"));

            bundles.Add(new ScriptBundle("~/js/admin/logistics/shippingarea/edit").Include(
                     "~/Modules/BntWeb.Logistics/Content/Scripts/shippingarea.edit.js"));

            bundles.Add(new ScriptBundle("~/Modules/BntWeb.Logistics/Content/Scripts/area").Include(
    "~/Modules/BntWeb.Logistics/Content/Scripts/area.data.js"));

        }
    }
}