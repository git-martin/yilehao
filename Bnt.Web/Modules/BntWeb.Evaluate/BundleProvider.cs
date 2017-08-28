using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using BntWeb.UI.Bundle;

namespace BntWeb.Evaluate
{
    public class BundleProvider : IBundleProvider
    {
        public void RegisterBundles(BundleCollection bundles)
        {
            //Js
            bundles.Add(new ScriptBundle("~/js/admin/evaluate/list").Include(
                      "~/Modules/BntWeb.Evaluate/Content/Scripts/evaluate.list.js"));
        }
    }
}