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

namespace BntWeb.SystemMessage
{
    public class BundleProvider : IBundleProvider
    {
        public void RegisterBundles(BundleCollection bundles)
        {
            //Js
            bundles.Add(new ScriptBundle("~/js/admin/systemMessage/list").Include(
                                        "~/Modules/BntWeb.SystemMessage/Content/Scripts/systemMessage.list.js"));
        }
    }
}