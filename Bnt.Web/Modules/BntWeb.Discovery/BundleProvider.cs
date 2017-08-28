
using BntWeb.UI.Bundle;
using System.Web.Optimization;

namespace BntWeb.Discovery
{
    public class BundleProvider: IBundleProvider
    {
        public void RegisterBundles(BundleCollection bundles)
        {
            //Js
            bundles.Add(new ScriptBundle("~/js/admin/discovery/list").Include(
                      "~/Modules/BntWeb.Discovery/Content/Scripts/discovery.list.js"));

            bundles.Add(new ScriptBundle("~/js/admin/discovery/edit").Include(
                        "~/Modules/BntWeb.Discovery/Content/Scripts/discovery.edit.js"));

           
        }
    }
}