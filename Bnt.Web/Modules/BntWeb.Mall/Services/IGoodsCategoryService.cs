using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BntWeb.Mall.Models;

namespace BntWeb.Mall.Services
{
    public interface IGoodsCategoryService : IDependency
    {
        /// <summary>
        /// 获取首页显示的分类
        /// </summary>
        /// <returns></returns>
        List<GoodsCategory> GetShowIndexCategories();

        /// <summary>
        /// 获取所有分类
        /// </summary>
        /// <returns></returns>
        List<GoodsCategory> GetCategories();

        /// <summary>
        /// 分页加载分类，包含子分类
        /// </summary>
        /// <param name="totalCount"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        List<GoodsCategory> LoadByPage(out int totalCount, int pageIndex = 1, int pageSize = 10);

        GoodsCategory GetCategoryById(Guid id);

        GoodsCategory GetCategoryByName(string name);

        Guid SaveCategory(GoodsCategory model);

        Guid CreateCategory(GoodsCategory model);

        Guid UpdateCategory(GoodsCategory model);

        bool DeleteCategory(GoodsCategory model);

        /// <summary>
        /// 分类下商品数量
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        int HasGoodsCount(Guid categoryId);

        #region 分类属性关联

        bool InsetCategoryRelation(GoodsCategoryRelation model);

        bool DeleteRelations(Guid goodsId);

        List<GoodsCategoryRelation> GetCategoryRelations(Guid goodsId);
        #endregion 
    }
}
