using System.Web.Optimization;
using BntWeb.UI.Bundle;

namespace BntWeb.Feedback
{
    public class BundleProvider : IBundleProvider
    {
        public void RegisterBundles(BundleCollection bundles)
        {
            //Js
            bundles.Add(new ScriptBundle("~/js/admin/feedbacks/list").Include(
                      "~/Modules/BntWeb.Feedback/Content/Scripts/feedback.list.js"));
        }
    }
}