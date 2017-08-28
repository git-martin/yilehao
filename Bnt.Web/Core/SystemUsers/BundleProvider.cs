/* 
    ======================================================================== 
        File name：        BundleProvider
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/5/25 16:45:54
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using BntWeb.UI.Bundle;

namespace BntWeb.Core.SystemUsers
{
    public class BundleProvider : IBundleProvider
    {
        public void RegisterBundles(BundleCollection bundles)
        {
            //Js
            bundles.Add(new ScriptBundle("~/js/admin/users/list").Include(
                      "~/Core/SystemUsers/Content/Scripts/user.list.js"));

            bundles.Add(new ScriptBundle("~/js/admin/roles/list").Include(
                      "~/Core/SystemUsers/Content/Scripts/role.list.js"));
        }
    }
}