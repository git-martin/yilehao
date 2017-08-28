/* 
    ======================================================================== 
        File name：		BundleProvider
        Module:			
        Author：		        罗嗣宝
        Create Time：		2016/6/17 15:21:48
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System.Web.Optimization;
using BntWeb.UI.Bundle;

namespace BntWeb.Mall
{
    public class BundleProvider : IBundleProvider
    {
        public void RegisterBundles(BundleCollection bundles)
        {
            //editable
            bundles.Add(new StyleBundle("~/css/admin/editable").Include(
                      "~/Resources/Admin/Css/jquery.gritter.css",
                      "~/Resources/Admin/Css/bootstrap-editable.css"));
            bundles.Add(new ScriptBundle("~/js/admin/editable").Include(
                      "~/Resources/Admin/Scripts/jquery.gritter.min.js",
                      "~/Resources/Admin/Scripts/x-editable/bootstrap-editable.min.js",
                      "~/Resources/Admin/Scripts/x-editable/ace-editable.min.js"));


            //Js
            bundles.Add(new ScriptBundle("~/js/admin/mall/goodstype/list").Include(
                      "~/Modules/BntWeb.Mall/Content/Scripts/goodstype.list.js"));

            bundles.Add(new ScriptBundle("~/js/admin/mall/goodsbrand/list").Include(
                      "~/Modules/BntWeb.Mall/Content/Scripts/goodsbrand.list.js"));

            bundles.Add(new ScriptBundle("~/js/admin/mall/attribute/list").Include(
                      "~/Modules/BntWeb.Mall/Content/Scripts/attribute.list.js"));

            bundles.Add(new ScriptBundle("~/js/admin/mall/goods/list").Include(
                      "~/Modules/BntWeb.Mall/Content/Scripts/goods.list.js",
                      "~/Modules/BntWeb.Mall/Content/Scripts/goods.list.category.js"));

            bundles.Add(new ScriptBundle("~/js/admin/mall/goodscategories/list").Include(
                      "~/Modules/BntWeb.Mall/Content/Scripts/goodscategory.list.js"));

            bundles.Add(new ScriptBundle("~/js/admin/mall/goodscategories/edit").Include(
                      "~/Modules/BntWeb.Mall/Content/Scripts/goodscategory.edit.js"));

            bundles.Add(new ScriptBundle("~/js/admin/mall/goods/edit").Include(
                      "~/Modules/BntWeb.Mall/Content/Scripts/goods.edit.js",
                      "~/Modules/BntWeb.Mall/Content/Scripts/goods.edit.category.js"));

            bundles.Add(new ScriptBundle("~/js/admin/mall/goodsshortage/list").Include(
                     "~/Modules/BntWeb.Mall/Content/Scripts/goodsshortage.list.js",
                     "~/Modules/BntWeb.Mall/Content/Scripts/goods.list.category.js"));

            bundles.Add(new ScriptBundle("~/js/admin/mall/goodsrecycle/list").Include(
                   "~/Modules/BntWeb.Mall/Content/Scripts/goodsrecycle.list.js"));

            bundles.Add(new ScriptBundle("~/js/admin/mall/goods/evaluate/list").Include(
                "~/Modules/BntWeb.Mall/Content/Scripts/evaluate.list.js"));

            bundles.Add(new ScriptBundle("~/js/admin/mall/goods/grouplist").Include(
                "~/Modules/BntWeb.Mall/Content/Scripts/goods.group.list.js"));
        }
    }
}