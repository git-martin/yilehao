/* 
    ======================================================================== 
        File name：        IGoodsService
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/7/1 16:03:46
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using BntWeb.Mall.ApiModels;
using BntWeb.Mall.Models;
using BntWeb.Mall.ViewModels;

namespace BntWeb.Mall.Services
{
    public interface IGoodsService : IDependency
    {
        /// <summary>
        /// 加载完整商品信息
        /// </summary>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        Goods LoadFullGoods(Guid goodsId);

        /// <summary>
        /// 加载完整单规格商品信息
        /// </summary>
        /// <param name="singleGoodsId"></param>
        /// <returns></returns>
        SingleGoods LoadFullSingleGoods(Guid singleGoodsId);

        /// <summary>
        /// 保存商品
        /// </summary>
        /// <param name="goods"></param>
        /// <param name="insert"></param>
        /// <returns></returns>
        bool SaveGoods(Goods goods, bool insert = true);

        /// <summary>
        /// 添加测试商品（仅测试使用）
        /// </summary>
        /// <param name="goods"></param>
        /// <returns></returns>
        bool AddGoodsForTest(Goods goods);
        /// <summary>
        /// 回收商品
        /// </summary>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        bool RecoveryGoods(Guid goodsId);

        /// <summary>
        /// 批量回收商品
        /// </summary>
        /// <param name="goodsIds"></param>
        /// <returns></returns>
        bool BatchRecoveryGoods(List<Guid> goodsIds);

        /// <summary>
        /// 还原商品
        /// </summary>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        bool RestoreGoods(Guid goodsId);

        /// <summary>
        /// 删除商品
        /// </summary>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        bool DeleteGoods(Guid goodsId);

        /// <summary>
        /// 批量删除商品
        /// </summary>
        /// <param name="goodsIds"></param>
        /// <returns></returns>
        bool BatchDeleteGoods(List<Guid> goodsIds);

        /// <summary>
        /// 设置商品状态
        /// </summary>
        /// <param name="goodId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        bool SetGoodsStatus(Guid goodId, GoodsStatus status);

        /// <summary>
        /// 获取所有分类
        /// </summary>
        /// <returns></returns>
        List<GoodsCategory> GetCategories();

        /// <summary>
        /// 根据分类获取商品
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="keyword"></param>
        /// <param name="sortType"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        List<Goods> GetGoodsByCategory(Guid? categoryId, string keyword, GoodsSortType sortType, int pageIndex, int pageSize, out int totalCount);

        List<Goods> GetGoodsByCategory(Guid? categoryId, string keyword, List<Guid> brands, decimal minPrice, decimal maxPrice,
            GoodsSortType sortType, int pageIndex, int pageSize, out int totalCount);

        List<GoodsView> GetGoodsByCategory(Guid? categoryId, string keyword, List<Guid> brands, List<AttributeModel> others, decimal minPrice, decimal maxPrice,
            GoodsSortType sortType, int pageIndex, int pageSize, out int totalCount);

        /// <summary>
        /// 取推荐商品, 新品，热卖，精品都算推荐
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        List<Goods> GetRecommendGoods(int pageIndex, int pageSize, out int totalCount);

        /// <summary>
        /// 取新品商品
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        List<Goods> GetNewGoods(int pageIndex, int pageSize, out int totalCount);

        /// <summary>
        /// 取精品商品
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        List<Goods> GetBestGoods(int pageIndex, int pageSize, out int totalCount);

        /// <summary>
        /// 取热卖商品
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        List<Goods> GetHotGoods(int pageIndex, int pageSize, out int totalCount);

        /// <summary>
        /// 预售列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        List<Goods> GetPresellGoods(int pageIndex, int pageSize, out int totalCount);

        /// <summary>
        /// 团购列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        List<Goods> GetGroupGoods(int pageIndex, int pageSize, out int totalCount);

        /// <summary>
        /// 获取商品属性数据
        /// </summary>
        /// <param name="goodId"></param>
        /// <returns></returns>
        List<GoodsAttiuteValues> GetGoodsAttributeValues(Guid goodId);

        #region
        /// <summary>
        /// 获取商品收藏列表
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        List<Models.Goods> LoadCollectGoodsByPage(string memberId, int pageIndex, int pageSize, out int totalCount);

        #endregion

        #region

        /// <summary>
        /// 获取商品浏览记录
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        List<BrowseViewModel> LoadBrowseGoodsByPage(string memberId, int pageIndex, int pageSize, out int totalCount);

        #endregion

        #region 单品

        /// <summary>
        /// 判断单品是否失效
        /// </summary>
        /// <param name="singleGoodsId"></param>
        /// <param name="originalPrice"></param>
        /// <returns></returns>
        bool CheckSingleGoodsIsInvalid(Guid singleGoodsId, decimal originalPrice);

        #endregion

        #region 商品评价
        /// <summary>
        /// 商品评价列表
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="totalCount"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        List<Evaluate.Models.Evaluate> GetGoodsEvaluatesListByPage(Guid goodsId, int pageIndex, int pageSize, out int totalCount);

        /// <summary>
        /// 最新评价
        /// </summary>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        List<Evaluate.Models.Evaluate> GetNew9Evaluates(Guid goodsId);

        #endregion

        /// <summary>
        /// 根据条件获取所有商品信息
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        List<Goods> GetList(Expression<Func<Goods, bool>> expression);
    }
}