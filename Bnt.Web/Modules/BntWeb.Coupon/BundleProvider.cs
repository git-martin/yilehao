using System.Web.Optimization;
using BntWeb.UI.Bundle;

namespace BntWeb.Coupon
{
    public class BundleProvider : IBundleProvider
    {
        public void RegisterBundles(BundleCollection bundles)
        {
            //Js
            bundles.Add(new ScriptBundle("~/js/admin/coupon/list").Include(
                      "~/Modules/BntWeb.Coupon/Content/Scripts/coupon.list.js"));
        }
    }
}