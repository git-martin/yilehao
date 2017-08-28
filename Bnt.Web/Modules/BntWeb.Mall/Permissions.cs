/* 
    ======================================================================== 
        File name：        Permissions
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/5/9 15:34:55
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/

using System.Collections.Generic;
using BntWeb.Security.Permissions;

namespace BntWeb.Mall
{
    public class Permissions : IPermissionProvider
    {
        private static readonly string CategoryKey = MallModule.DisplayName;

        public const string ManageGoodsTypeKey = "BntWeb-Mall-ManageGoodsType";
        public static readonly Permission ManageGoodsType = new Permission { Description = "商品类型管理", Name = ManageGoodsTypeKey, Category = CategoryKey };

        public const string ManageGoodsBrandKey = "BntWeb-Mall-ManageGoodsBrand";
        public static readonly Permission ManageGoodsBrand = new Permission { Description = "品牌管理", Name = ManageGoodsBrandKey, Category = CategoryKey };

        public const string ManageGoodsKey = "BntWeb-Mall-ManageGoods";
        public static readonly Permission ManageGoods = new Permission { Description = "商品管理", Name = ManageGoodsKey, Category = CategoryKey };

        public const string ManageGoodsCategorieKey = "BntWeb-Mall-ManageGoodsCategorie";
        public static readonly Permission ManageGoodsCategorie = new Permission { Description = "商品分类管理", Name = ManageGoodsCategorieKey, Category = CategoryKey };

        public const string ManageTagKey = "BntWeb-Mall-ManageTag";
        public static readonly Permission ManageTag = new Permission { Description = "管理标签", Name = ManageTagKey, Category = CategoryKey };

        public const string GoodsToExeclKey = "BntWeb-Mall-GoodsToExecl";

        public static readonly Permission GoodsToExecl = new Permission
        {
            Description = "商品导出",
            Name = GoodsToExeclKey,
            Category = CategoryKey
        };

        public int Position => MallModule.Position;

        public string Category => CategoryKey;

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ManageGoodsType,
                ManageGoodsBrand,
                ManageGoods,
                ManageGoodsCategorie,
                ManageTag,
                GoodsToExecl
            };
        }
    }
}