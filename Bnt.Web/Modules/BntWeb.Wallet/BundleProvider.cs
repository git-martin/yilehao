using System.Web.Optimization;
using BntWeb.UI.Bundle;

namespace BntWeb.Wallet
{
    public class BundleProvider : IBundleProvider
    {
        public void RegisterBundles(BundleCollection bundles)
        {
            //Js
            bundles.Add(new ScriptBundle("~/js/admin/Wallet/list").Include(
                      "~/Modules/BntWeb.Wallet/Content/Scripts/crashapply.list.js"));
            bundles.Add(new ScriptBundle("~/js/admin/Wallet/bill/list").Include(
                      "~/Modules/BntWeb.Wallet/Content/Scripts/bill.list.js"));
        }
    }
}