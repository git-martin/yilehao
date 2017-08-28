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

namespace BntWeb.Core.SystemSettings
{
    public class BundleProvider : IBundleProvider
    {
        public void RegisterBundles(BundleCollection bundles)
        {
            //Js
            bundles.Add(new ScriptBundle("~/js/admin/sms/list").Include(
                      "~/Core/SystemSettings/Content/Scripts/sms.list.js"));

            bundles.Add(new ScriptBundle("~/js/admin/logs/list").Include(
                      "~/Core/SystemSettings/Content/Scripts/log.list.js"));

            bundles.Add(new ScriptBundle("~/js/admin/district/index").Include(
                      "~/Core/SystemSettings/Content/Scripts/district.index.js"));

            bundles.Add(new ScriptBundle("~/js/admin/files/list").Include(
                      "~/Core/SystemSettings/Content/Scripts/file.list.js"));

            bundles.Add(new ScriptBundle("~/js/admin/release/list").Include(
                      "~/Core/SystemSettings/Content/Scripts/soft.release.list.js"));

            bundles.Add(new ScriptBundle("~/js/admin/colorbox").Include(
                      "~/Resources/Admin/Scripts/jquery.colorbox-min.js"));

            bundles.Add(new ScriptBundle("~/js/admin/backup/list").Include(
                      "~/Core/SystemSettings/Content/Scripts/backup.list.js"));

            //css
            bundles.Add(new StyleBundle("~/css/admin/colorbox").Include(
                      "~/Resources/Admin/Css/colorbox.css"));
        }
    }
}