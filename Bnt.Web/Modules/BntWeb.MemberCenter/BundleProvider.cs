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

using System.Web.Optimization;
using BntWeb.UI.Bundle;

namespace BntWeb.MemberCenter
{
    public class BundleProvider : IBundleProvider
    {
        public void RegisterBundles(BundleCollection bundles)
        {
            //Js
            bundles.Add(new ScriptBundle("~/js/admin/members/list").Include(
                      "~/Modules/BntWeb.MemberCenter/Content/Scripts/member.list.js"));
        }
    }
}