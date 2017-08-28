using System.Web.Optimization;
using BntWeb.UI.Bundle;

namespace BntWeb.Feedback
{
    public class BundleProvider : IBundleProvider
    {
        public void RegisterBundles(BundleCollection bundles)
        {
            //Js
            bundles.Add(new ScriptBundle("~/js/admin/singlepages/list").Include(
                      "~/Modules/BntWeb.SinglePage/Content/Scripts/singlepage.list.js"));
        }
    }
}