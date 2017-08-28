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

namespace BntWeb.Carousel
{
    public class BundleProvider : IBundleProvider
    {
        public void RegisterBundles(BundleCollection bundles)
        {
            //Js
            bundles.Add(new ScriptBundle("~/js/admin/carousels/grouplist").Include(
                      "~/Modules/BntWeb.Carousel/Content/Scripts/carousel.group.list.js"));

            bundles.Add(new ScriptBundle("~/js/admin/carousels/itemlist").Include(
                      "~/Modules/BntWeb.Carousel/Content/Scripts/carousel.item.list.js"));
        }
    }
}