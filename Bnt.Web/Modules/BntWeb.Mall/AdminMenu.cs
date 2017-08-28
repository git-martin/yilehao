/* 
    ======================================================================== 
        File name：        AdminMenu
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/6/16 11:52:13
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BntWeb.Tag;
using BntWeb.UI.Navigation;

namespace BntWeb.Mall
{
    public class AdminMenu : INavigationProvider
    {
        //public Localizer T { get; set; }
        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add(MallModule.Key, MallModule.DisplayName, MallModule.Position.ToString(), BuildMenu, new List<string> { "icon-shopping-cart" });
        }

        private void BuildMenu(NavigationItemBuilder menu)
        {
            menu.Add(MallModule.Key + "-GoodsTypeList", "商品类型", "10",
                item => item
                    .Action("List", "GoodsType", new { area = MallModule.Area })
                    .Permission(Permissions.ManageGoodsType)
                );
            menu.Add(MallModule.Key + "-GoodsBrandList", "品牌管理", "15",
                item => item
                    .Action("List", "GoodsBrand", new { area = MallModule.Area })
                    .Permission(Permissions.ManageGoodsBrand)
                );
            menu.Add(MallModule.Key + "-GoodsCategoryList", "商品分类", "20",
                item => item
                    .Action("List", "GoodsCategory", new { area = MallModule.Area })
                    .Permission(Permissions.ManageGoodsCategorie)
                );
            menu.Add(MallModule.Key + "-GoodsList", "商品列表", "30",
                item => item
                    .Action("List", "Goods", new { area = MallModule.Area })
                    .Permission(Permissions.ManageGoods)
                );
            menu.Add(MallModule.Key + "-GoodsShortageList", "缺货商品", "30",
               item => item
                   .Action("List", "GoodsShortage", new { area = MallModule.Area })
                   .Permission(Permissions.ManageGoods)
               );
            menu.Add(MallModule.Key + "-GroupGoodsList", "团购列表", "30",
              item => item
                  .Action("GroupList", "Goods", new { area = MallModule.Area })
                  .Permission(Permissions.ManageGoods)
              );

            menu.Add(MallModule.Key + "-TagsList", "商品标签", "30",
                item => item
                    .Action("Index", "Admin", new { area = TagModule.Area, sourceType = "Goods", moduleKey = MallModule.Key })
                    .Permission(Permissions.ManageTag)
                );

            menu.Add(MallModule.Key + "-GoodsRecycleList", "商品回收", "30",
             item => item
                 .Action("List", "GoodsRecycle", new { area = MallModule.Area })
                 .Permission(Permissions.ManageGoods)
             );
        }
    }
}