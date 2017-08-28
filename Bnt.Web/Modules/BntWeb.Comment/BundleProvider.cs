/* 
    ======================================================================== 
        File name：        BundleProvider
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/6/21 13:24:07
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

namespace BntWeb.Comment
{
    public class BundleProvider : IBundleProvider
    {
        public void RegisterBundles(BundleCollection bundles)
        {
            //Js
            bundles.Add(new ScriptBundle("~/js/admin/comments/list").Include(
                      "~/Modules/BntWeb.Comment/Content/Scripts/comment.list.js"));
        }
    }
}