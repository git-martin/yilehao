/* 
    ======================================================================== 
        File name：        GoodsService
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/7/5 17:04:39
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using BntWeb.ContentMarkup.Models;
using BntWeb.Data;
using BntWeb.FileSystems.Media;
using BntWeb.Logging;
using BntWeb.Mall.ApiModels;
using BntWeb.Mall.Models;
using BntWeb.Mall.ViewModels;
using BntWeb.OrderProcess;
using BntWeb.Security;
using BntWeb.Utility.Extensions;
using Microsoft.Ajax.Utilities;
using Microsoft.SqlServer.Server;

namespace BntWeb.Mall.Services
{
    public class GoodsService : IGoodsService
    {
        private readonly IStorageFileService _storageFileService;
        public GoodsService(IStorageFileService storageFileService)
        {
            _storageFileService = storageFileService;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public Goods LoadFullGoods(Guid goodsId)
        {
            using (var dbContext = new MallDbContext())
            {
                var goods = dbContext.Goods.FirstOrDefault(g => g.Id.Equals(goodsId));
                if (goods == null) return null;

                goods.SingleGoods = dbContext.SingleGoods.Where(sg => sg.GoodsId.Equals(goodsId)).ToList();

                foreach (var singleGoods in goods.SingleGoods)
                {
                    //商品Id作为SourceType，在编辑保存的时候，根据这个值批量删除历史文件关联
                    singleGoods.Image = _storageFileService.GetFiles(singleGoods.Id, MallModule.Instance.InnerKey, goods.Id.ToString()).FirstOrDefault()?.Simplified() ?? _storageFileService.GetFiles(goodsId, MallModule.Key, "MainImage").FirstOrDefault()?.Simplified();
                    singleGoods.Attributes = dbContext.SingleGoodsAttributes.Where(sga => sga.SingleGoodsId.Equals(singleGoods.Id)).OrderBy(sga => sga.AttributeId).ToList();
                }
                return goods;
            }
        }

        public SingleGoods LoadFullSingleGoods(Guid singleGoodsId)
        {
            using (var dbContext = new MallDbContext())
            {
                var singleGoods = dbContext.SingleGoods.Include(sg => sg.Goods).FirstOrDefault(g => g.Id.Equals(singleGoodsId));
                if (singleGoods == null) return null;

                singleGoods.Attributes = dbContext.SingleGoodsAttributes.Where(sga => sga.SingleGoodsId.Equals(singleGoods.Id)).ToList();
                return singleGoods;
            }
        }

        /// <summary>
        /// 添加测试商品（仅测试使用）
        /// </summary>
        /// <param name="goods"></param>
        /// <returns></returns>
        public bool AddGoodsForTest(Goods goods)
        {
            using (var dbContext = new MallDbContext())
            {

                goods.Id = KeyGenerator.GetGuidKey();
                dbContext.Goods.Add(goods);

                //dbContext.SingleGoods.AddRange(goods.SingleGoods);
                foreach (var singleGoods in goods.SingleGoods)
                {
                    singleGoods.Id = KeyGenerator.GetGuidKey();
                    dbContext.SingleGoods.Add(singleGoods);
                    foreach (var attr in singleGoods.Attributes)
                    {
                        attr.Id = KeyGenerator.GetGuidKey();
                        dbContext.SingleGoodsAttributes.Add(attr);
                    }
                    //dbContext.SingleGoodsAttributes.AddRange(singleGoods.Attributes);
                }

                if (dbContext.SaveChanges() > 0)
                {
                    return true;
                }
                else
                    return false;
            }
        }
        public bool SaveGoods(Goods goods, bool insert = true)
        {
            using (var dbContext = new MallDbContext())
            {
                if (insert)
                    dbContext.Goods.Add(goods);
                else
                {
                    dbContext.Goods.Attach(goods);
                    dbContext.Entry(goods).State = EntityState.Modified;
                }

                var singleGoodsList = dbContext.SingleGoods.Where(sg => sg.GoodsId.Equals(goods.Id)).ToList();
                foreach (var singleGoods in singleGoodsList)
                {
                    var attributes = dbContext.SingleGoodsAttributes.Where(sga => sga.SingleGoodsId.Equals(singleGoods.Id)).ToList();
                    attributes.ForEach(m => dbContext.Entry(m).State = EntityState.Deleted);
                }
                singleGoodsList.ForEach(m => dbContext.Entry(m).State = EntityState.Deleted);

                dbContext.SingleGoods.AddRange(goods.SingleGoods);
                foreach (var singleGoods in goods.SingleGoods)
                {
                    dbContext.SingleGoodsAttributes.AddRange(singleGoods.Attributes);
                }


                if (dbContext.SaveChanges() > 0)
                {
                    //删除历史单品图片
                    _storageFileService.DeleteBySourceType(goods.Id.ToString());
                    //关联单品的图片
                    foreach (var singleGoods in goods.SingleGoods)
                    {
                        _storageFileService.AssociateFile(singleGoods.Id, MallModule.Instance.InnerKey, MallModule.Instance.InnerDisplayName, singleGoods.Image.Id, goods.Id.ToString());
                    }
                    return true;
                }
                else
                    return false;
            }
        }

        public bool RecoveryGoods(Guid goodsId)
        {
            using (var dbContext = new MallDbContext())
            {
                var goods = dbContext.Goods.FirstOrDefault(g => g.Id.Equals(goodsId));
                if (goods != null)
                {
                    goods.Status = GoodsStatus.Delete;
                    dbContext.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public bool BatchRecoveryGoods(List<Guid> goodsIds)
        {
            using (var dbContext = new MallDbContext())
            {
                var arrGoodsIds = goodsIds.ToArray();
                var goodsList = dbContext.Goods.Where(g => arrGoodsIds.Contains(g.Id)).ToList();
                if (goodsList.Any())
                {
                    foreach (var goods in goodsList)
                    {
                        goods.Status = GoodsStatus.Delete;
                    }

                    dbContext.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public bool RestoreGoods(Guid goodsId)
        {
            using (var dbContext = new MallDbContext())
            {
                var goods = dbContext.Goods.FirstOrDefault(g => g.Id.Equals(goodsId));
                if (goods != null)
                {
                    goods.Status = GoodsStatus.NotInSale;
                    dbContext.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public bool DeleteGoods(Guid goodsId)
        {
            var goods = LoadFullGoods(goodsId);
            if (goods == null) return true;

            using (var dbContext = new MallDbContext())
            {
                foreach (var singleGoods in goods.SingleGoods)
                {
                    //单品属性
                    var attributes = dbContext.SingleGoodsAttributes.Where(sga => sga.SingleGoodsId.Equals(singleGoods.Id)).ToList();
                    attributes.ForEach(m => dbContext.Entry(m).State = EntityState.Deleted);
                }
                //单品信息
                goods.SingleGoods.ForEach(m => dbContext.Entry(m).State = EntityState.Deleted);
                //商品分类关联关系
                var categories = dbContext.GoodsCategoryRelations.Where(sga => sga.GoodsId.Equals(goodsId)).ToList();
                categories.ForEach(m => dbContext.Entry(m).State = EntityState.Deleted);
                //购物车
                var carts = dbContext.Carts.Where(me => me.GoodsId.Equals(goodsId)).ToList();
                carts.ForEach(m => dbContext.Entry(m).State = EntityState.Deleted);
                //商品相关标记
                var mrkupsList = dbContext.Markups.Where(me => me.SourceId.Equals(goodsId) && me.ModuleKey == MallModule.Key).ToList();
                mrkupsList.ForEach(m => dbContext.Entry(m).State = EntityState.Deleted);
                //商品主信息
                dbContext.Entry(goods).State = EntityState.Deleted;
                dbContext.SaveChanges();
                //商品图片
                _storageFileService.DisassociateFile(goodsId, MallModule.Key);
                return true;
            }
        }

        public bool BatchDeleteGoods(List<Guid> goodsIds)
        {
            using (var dbContext = new MallDbContext())
            {
                var arrGoodsIds = goodsIds.ToArray();
                var goodsList = dbContext.Goods.Where(g => arrGoodsIds.Contains(g.Id)).ToList();
                if (goodsList.Any())
                {
                    foreach (var goods in goodsList)
                    {
                        goods.SingleGoods = dbContext.SingleGoods.Where(sg => sg.GoodsId.Equals(goods.Id)).ToList();

                        foreach (var singleGoods in goods.SingleGoods)
                        {
                            //单品属性
                            var attributes = dbContext.SingleGoodsAttributes.Where(sga => sga.SingleGoodsId.Equals(singleGoods.Id)).ToList();
                            attributes.ForEach(m => dbContext.Entry(m).State = EntityState.Deleted);
                        }
                        //单品信息
                        goods.SingleGoods.ForEach(m => dbContext.Entry(m).State = EntityState.Deleted);
                        //商品分类关联关系
                        var categories = dbContext.GoodsCategoryRelations.Where(sga => sga.GoodsId.Equals(goods.Id)).ToList();
                        categories.ForEach(m => dbContext.Entry(m).State = EntityState.Deleted);
                        //购物车
                        var carts = dbContext.Carts.Where(me => me.GoodsId.Equals(goods.Id)).ToList();
                        carts.ForEach(m => dbContext.Entry(m).State = EntityState.Deleted);
                        //商品相关标记
                        var mrkupsList = dbContext.Markups.Where(me => me.SourceId.Equals(goods.Id) && me.ModuleKey == MallModule.Key).ToList();
                        mrkupsList.ForEach(m => dbContext.Entry(m).State = EntityState.Deleted);
                        //商品主信息
                        dbContext.Entry(goods).State = EntityState.Deleted;
                        //商品图片
                        _storageFileService.DisassociateFile(goods.Id, MallModule.Key);
                    }

                    dbContext.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="goodId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool SetGoodsStatus(Guid goodId, GoodsStatus status)
        {
            using (var dbContext = new MallDbContext())
            {
                var goods = dbContext.Goods.FirstOrDefault(me => me.Id == goodId);

                if (goods != null)
                {
                    goods.Status = status;
                    dbContext.SaveChanges();

                    Logger.Operation($"[{goods.Name}]设置状态为：{goods.Status.Description()}", MallModule.Instance, SecurityLevel.Normal);
                    return true;
                }
                return false;

            }
        }

        public List<GoodsCategory> GetCategories()
        {
            using (var dbContext = new MallDbContext())
            {
                return dbContext.GoodsCategories.OrderByDescending(me => me.Sort).ToList();
            }
        }

        /// <summary>
        /// 取分类商品
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="keyword"></param>
        /// <param name="sortType"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<Goods> GetGoodsByCategory(Guid? categoryId, string keyword, GoodsSortType sortType, int pageIndex, int pageSize, out int totalCount)
        {
            using (var dbContext = new MallDbContext())
            {
                var query = dbContext.Goods.Where(me => me.Status > 0);
                if (categoryId != null && categoryId != Guid.Empty)
                    query = query.Where(me => me.CategoryId == categoryId);
                if (!string.IsNullOrWhiteSpace(keyword))
                    query = query.Where(me => me.Name.Contains(keyword));
                switch (sortType)
                {
                    case GoodsSortType.Degault:
                        query = query.OrderByDescending(me => me.Sort).ThenByDescending(me => me.CreateTime);
                        break;
                    case GoodsSortType.PriceLow:
                        query = query.OrderBy(me => me.ShopPrice).ThenByDescending(me => me.Sort).ThenByDescending(me => me.CreateTime);
                        break;
                    case GoodsSortType.PriceHigh:
                        query = query.OrderByDescending(me => me.ShopPrice).ThenByDescending(me => me.Sort).ThenByDescending(me => me.CreateTime);
                        break;
                    case GoodsSortType.SalesVolumeHigh:
                        query = query.OrderByDescending(me => me.SalesVolume).ThenByDescending(me => me.Sort).ThenByDescending(me => me.CreateTime);
                        break;
                    default:
                        query = query.OrderByDescending(me => me.Sort).ThenByDescending(me => me.CreateTime);
                        break;
                }

                totalCount = query.Count();
                return query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
        }
        /// <summary>
        /// 商品信息查询
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="keyword"></param>
        /// <param name="brands"></param>
        /// <param name="minPrice"></param>
        /// <param name="maxPrice"></param>
        /// <param name="sortType"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<Goods> GetGoodsByCategory(Guid? categoryId, string keyword, List<Guid> brands, decimal minPrice, decimal maxPrice, GoodsSortType sortType, int pageIndex, int pageSize, out int totalCount)
        {
            using (var dbContext = new MallDbContext())
            {
                var query = from g in dbContext.Goods
                            join gr in dbContext.GoodsCategoryRelations on g.Id equals gr.GoodsId into temp
                            from gr in temp.DefaultIfEmpty()
                            join gc in dbContext.GoodsCategories on g.CategoryId equals gc.Id
                            where g.Status == GoodsStatus.InSale
                            select new { g, gr.CategoryId, gc.MergerId }
                ;
                //var query = dbContext.Goods.Where(me => me.Status > 0);
                //分类
                if (categoryId != null && categoryId != Guid.Empty)
                    query = query.Where(me => me.g.CategoryId == categoryId || me.CategoryId == categoryId || me.MergerId.Contains(categoryId.ToString()));

                //价格
                if (minPrice > 0)
                    query = query.Where(me => me.g.ShopPrice >= minPrice);
                if (maxPrice > 0)
                    query = query.Where(me => me.g.ShopPrice <= maxPrice);
                //关键词
                if (!string.IsNullOrWhiteSpace(keyword))
                    query = query.Where(me => me.g.Name.Contains(keyword));
                //品牌
                if (brands != null && brands.Count > 0)
                    query = query.Where(me => brands.Contains((Guid)me.g.BrandId));

                //排序
                switch (sortType)
                {
                    case GoodsSortType.Degault:
                        query = query.OrderByDescending(me => me.g.Sort).ThenByDescending(me => me.g.CreateTime);
                        break;
                    case GoodsSortType.PriceLow:
                        query = query.OrderBy(me => me.g.ShopPrice).ThenByDescending(me => me.g.Sort).ThenByDescending(me => me.g.CreateTime);
                        break;
                    case GoodsSortType.PriceHigh:
                        query = query.OrderByDescending(me => me.g.ShopPrice).ThenByDescending(me => me.g.Sort).ThenByDescending(me => me.g.CreateTime);
                        break;
                    case GoodsSortType.SalesVolumeHigh:
                        query = query.OrderByDescending(me => me.g.SalesVolume).ThenByDescending(me => me.g.Sort).ThenByDescending(me => me.g.CreateTime);
                        break;
                    default:
                        query = query.OrderByDescending(me => me.g.Sort).ThenByDescending(me => me.g.CreateTime);
                        break;
                }

                totalCount = query.Count();
                return query.Select(x => x.g).DistinctBy(x => x.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
        }

        /// <summary>
        /// 【视图查询】商品信息查询
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="keyword"></param>
        /// <param name="brands"></param>
        /// <param name="minPrice"></param>
        /// <param name="maxPrice"></param>
        /// <param name="sortType"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<Goods> GetGoodsByCategory_View(Guid? categoryId, string keyword, List<Guid> brands, decimal minPrice, decimal maxPrice, GoodsSortType sortType, int pageIndex, int pageSize, out int totalCount)
        {
            using (var dbContext = new MallDbContext())
            {
                var query = from v in dbContext.GoodsCategoryView select v;
                ;
                //var query = dbContext.Goods.Where(me => me.Status > 0);
                //分类
                if (categoryId != null && categoryId != Guid.Empty)
                    query = query.Where(me => me.CategoryId == categoryId || me.ExtentCategoryId == categoryId || me.MergerId.Contains(categoryId.ToString()));

                //价格
                if (minPrice > 0)
                    query = query.Where(me => me.ShopPrice >= minPrice);
                if (maxPrice > 0)
                    query = query.Where(me => me.ShopPrice <= maxPrice);
                //关键词
                if (!string.IsNullOrWhiteSpace(keyword))
                    query = query.Where(me => me.Name.Contains(keyword));
                //品牌
                if (brands != null && brands.Count > 0)
                    query = query.Where(me => brands.Contains((Guid)me.BrandId));

                //排序
                switch (sortType)
                {
                    case GoodsSortType.Degault:
                        query = query.OrderByDescending(me => me.Sort).ThenByDescending(me => me.CreateTime);
                        break;
                    case GoodsSortType.PriceLow:
                        query = query.OrderBy(me => me.ShopPrice).ThenByDescending(me => me.Sort).ThenByDescending(me => me.CreateTime);
                        break;
                    case GoodsSortType.PriceHigh:
                        query = query.OrderByDescending(me => me.ShopPrice).ThenByDescending(me => me.Sort).ThenByDescending(me => me.CreateTime);
                        break;
                    case GoodsSortType.SalesVolumeHigh:
                        query = query.OrderByDescending(me => me.SalesVolume).ThenByDescending(me => me.Sort).ThenByDescending(me => me.CreateTime);
                        break;
                    default:
                        query = query.OrderByDescending(me => me.Sort).ThenByDescending(me => me.CreateTime);
                        break;
                }

                totalCount = query.Count();
                return query.Select(x => new Goods
                {
                    Id = x.Id,
                    Name = x.Name,
                    GoodsNo = x.GoodsNo,
                    BrandId = x.BrandId,
                    GoodsType = x.GoodsType,
                    CategoryId = x.CategoryId,
                    OriginalPrice = x.OriginalPrice,
                    ShopPrice = x.ShopPrice,
                    IsNew = x.IsNew,
                    IsBest = x.IsBest,
                    IsHot = x.IsHot,
                    CreateTime = x.CreateTime,
                    LastUpdateTime = x.LastUpdateTime,
                    Stock = x.Stock,
                    PaymentAmount = x.PaymentAmount,
                    SalesVolume = x.SalesVolume,
                    EvaluateCount = x.EvaluateCount,
                    ViewCount = x.ViewCount,
                    Status = x.Status,
                    Sort = x.Sort,
                    FreeShipping = x.FreeShipping
                }).DistinctBy(x => x.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
        }
        /// <summary>
        /// 商品信息属性查询
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="keyword"></param>
        /// <param name="brands"></param>
        /// <param name="others"></param>
        /// <param name="minPrice"></param>
        /// <param name="maxPrice"></param>
        /// <param name="sortType"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<GoodsView> GetGoodsByCategory(Guid? categoryId, string keyword, List<Guid> brands, List<AttributeModel> others, decimal minPrice, decimal maxPrice, GoodsSortType sortType, int pageIndex, int pageSize, out int totalCount)
        {
            using (var dbContext = new MallDbContext())
            {
                var query = from g in dbContext.GoodsViews
                            join gr in dbContext.GoodsCategoryRelations on g.Id equals gr.GoodsId into temp
                            from gr in temp.DefaultIfEmpty()
                            join gc in dbContext.GoodsCategories on g.CategoryId equals gc.Id
                            where g.Status == GoodsStatus.InSale
                            select new ExtendGoodsViewModelnew { GoodsView = g, MergerCategoryId = gc.MergerId, ExtendCategoryId = gr.CategoryId }
                ;
                //var query = dbContext.GoodsViews.Where(me => me.Status != GoodsStatus.Delete);
                //分类
                if (categoryId != null && categoryId != Guid.Empty)
                {
                    var strCategoryId = categoryId.ToString();
                    query = query.Where(me => me.MergerCategoryId.Contains(strCategoryId) || me.ExtendCategoryId == categoryId);
                }
                //价格
                if (minPrice > 0)
                    query = query.Where(me => me.GoodsView.ShopPrice >= minPrice);
                if (maxPrice > 0)
                    query = query.Where(me => me.GoodsView.ShopPrice <= maxPrice);
                //关键词
                if (!string.IsNullOrWhiteSpace(keyword))
                    query = query.Where(me => me.GoodsView.Name.Contains(keyword));
                //品牌
                if (brands != null && brands.Count > 0)
                    query = query.Where(me => brands.Contains((Guid)me.GoodsView.BrandId));
                //其他属性
                if (others != null && others.Count > 0)
                {

                    foreach (var attribute in others)
                    {
                        //寻找符合条件的单品
                        var values = attribute.AttributeValues.Split(',');

                        query = query.Where(me => !(me.GoodsView.AttributeId == attribute.AttributeId && !values.Contains(me.GoodsView.AttributeValue)));
                    }
                }

                //排序
                switch (sortType)
                {
                    case GoodsSortType.Degault:
                        query = query.OrderByDescending(me => me.GoodsView.Sort).ThenByDescending(me => me.GoodsView.CreateTime);
                        break;
                    case GoodsSortType.PriceLow:
                        query = query.OrderBy(me => me.GoodsView.ShopPrice).ThenByDescending(me => me.GoodsView.Sort).ThenByDescending(me => me.GoodsView.CreateTime);
                        break;
                    case GoodsSortType.PriceHigh:
                        query = query.OrderByDescending(me => me.GoodsView.ShopPrice).ThenByDescending(me => me.GoodsView.Sort).ThenByDescending(me => me.GoodsView.CreateTime);
                        break;
                    case GoodsSortType.SalesVolumeHigh:
                        query = query.OrderByDescending(me => me.GoodsView.SalesVolume).ThenByDescending(me => me.GoodsView.Sort).ThenByDescending(me => me.GoodsView.CreateTime);
                        break;
                    default:
                        query = query.OrderByDescending(me => me.GoodsView.Sort).ThenByDescending(me => me.GoodsView.CreateTime);
                        break;
                }

                totalCount = query.Select(x => x.GoodsView.Id).Distinct().Count();
                return query.Select(x => x.GoodsView).DistinctBy(x => x.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
        }

        /// <summary>
        /// 【视图查询】商品信息属性查询
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="keyword"></param>
        /// <param name="brands"></param>
        /// <param name="others"></param>
        /// <param name="minPrice"></param>
        /// <param name="maxPrice"></param>
        /// <param name="sortType"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<GoodsView> GetGoodsByCategory_View(Guid? categoryId, string keyword, List<Guid> brands, List<AttributeModel> others, decimal minPrice, decimal maxPrice, GoodsSortType sortType, int pageIndex, int pageSize, out int totalCount)
        {
            using (var dbContext = new MallDbContext())
            {
                var query = from v in dbContext.SingleGoodsCategoryView
                            where v.CategoryId == categoryId
                            select new ExtendGoodsViewModelnew
                            {
                                GoodsView =
                            {
                                Id=v.Id,
                                Name = v.Name,
                                GoodsNo = v.GoodsNo,
                                BrandId =v.BrandId,
                                GoodsType = v.GoodsType,
                                CategoryId = v.CategoryId,
                                OriginalPrice = v.OriginalPrice,
                                ShopPrice = v.ShopPrice,
                                IsNew = v.IsNew,
                                IsBest=v.IsBest,
                                IsHot = v.IsHot,
                                CreateTime = v.CreateTime,
                                LastUpdateTime = v.LastUpdateTime,
                                Stock=v.Stock,
                                PaymentAmount = v.PaymentAmount,
                                SalesVolume = v.SalesVolume,
                                EvaluateCount = v.EvaluateCount,
                                ViewCount = v.ViewCount,
                                Status = v.Status,
                                Sort = v.Sort,
                                FreeShipping = v.FreeShipping,
                                SingleGoodsId = v.SingleGoodsId,
                                AttributeId = v.AttributeId,
                                AttributeValue = v.AttributeValue
                            },
                                ExtendCategoryId = v.ExtentCategoryId
                            }
                ;
                //var query = dbContext.GoodsViews.Where(me => me.Status != GoodsStatus.Delete);
                //分类
                if (categoryId != null && categoryId != Guid.Empty)
                    query = query.Where(me => me.GoodsView.CategoryId == categoryId || me.ExtendCategoryId == categoryId);
                //价格
                if (minPrice > 0)
                    query = query.Where(me => me.GoodsView.ShopPrice >= minPrice);
                if (maxPrice > 0)
                    query = query.Where(me => me.GoodsView.ShopPrice <= maxPrice);
                //关键词
                if (!string.IsNullOrWhiteSpace(keyword))
                    query = query.Where(me => me.GoodsView.Name.Contains(keyword));
                //品牌
                if (brands != null && brands.Count > 0)
                    query = query.Where(me => brands.Contains((Guid)me.GoodsView.BrandId));
                //其他属性
                if (others != null && others.Count > 0)
                {
                    foreach (var attribute in others)
                    {
                        //寻找符合条件的单品
                        var values = attribute.AttributeValues.Split(',');

                        query = query.Where(me => !(me.GoodsView.AttributeId == attribute.AttributeId && !values.Contains(me.GoodsView.AttributeValue)));
                    }
                }

                //排序
                switch (sortType)
                {
                    case GoodsSortType.Degault:
                        query = query.OrderByDescending(me => me.GoodsView.Sort).ThenByDescending(me => me.GoodsView.CreateTime);
                        break;
                    case GoodsSortType.PriceLow:
                        query = query.OrderBy(me => me.GoodsView.ShopPrice).ThenByDescending(me => me.GoodsView.Sort).ThenByDescending(me => me.GoodsView.CreateTime);
                        break;
                    case GoodsSortType.PriceHigh:
                        query = query.OrderByDescending(me => me.GoodsView.ShopPrice).ThenByDescending(me => me.GoodsView.Sort).ThenByDescending(me => me.GoodsView.CreateTime);
                        break;
                    case GoodsSortType.SalesVolumeHigh:
                        query = query.OrderByDescending(me => me.GoodsView.SalesVolume).ThenByDescending(me => me.GoodsView.Sort).ThenByDescending(me => me.GoodsView.CreateTime);
                        break;
                    default:
                        query = query.OrderByDescending(me => me.GoodsView.Sort).ThenByDescending(me => me.GoodsView.CreateTime);
                        break;
                }

                totalCount = query.Select(x => x.GoodsView.Id).Distinct().Count();
                return query.Select(x => x.GoodsView).DistinctBy(x => x.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
        }

        /// <summary>
        /// 取推荐商品, 新品，热卖，精品都算推荐
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<Goods> GetRecommendGoods(int pageIndex, int pageSize, out int totalCount)
        {
            using (var dbContext = new MallDbContext())
            {
                var query = dbContext.Goods.Where(me => (me.IsNew || me.IsHot || me.IsBest) && me.Status == GoodsStatus.InSale)
                    .OrderByDescending(me => me.Sort).ThenByDescending(me => me.CreateTime);

                totalCount = query.Count();
                return query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
        }

        /// <summary>
        /// 取新品商品
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<Goods> GetNewGoods(int pageIndex, int pageSize, out int totalCount)
        {
            using (var dbContext = new MallDbContext())
            {
                var query = dbContext.Goods.Where(me => me.IsNew && me.Status == GoodsStatus.InSale)
                    .OrderByDescending(me => me.Sort).ThenByDescending(me => me.CreateTime);

                totalCount = query.Count();
                return query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
        }

        /// <summary>
        /// 取精品商品
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<Goods> GetBestGoods(int pageIndex, int pageSize, out int totalCount)
        {
            using (var dbContext = new MallDbContext())
            {
                var query = dbContext.Goods.Where(me => me.IsBest && me.Status == GoodsStatus.InSale)
                    .OrderByDescending(me => me.Sort).ThenByDescending(me => me.CreateTime);

                totalCount = query.Count();
                return query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
        }

        /// <summary>
        /// 取热卖商品
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<Goods> GetHotGoods(int pageIndex, int pageSize, out int totalCount)
        {
            using (var dbContext = new MallDbContext())
            {
                var query = dbContext.Goods.Where(me => me.IsHot && me.Status == GoodsStatus.InSale)
                    .OrderByDescending(me => me.Sort).ThenByDescending(me => me.CreateTime);

                totalCount = query.Count();
                return query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
        }



        /// <summary>
        /// 预售列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<Goods> GetPresellGoods(int pageIndex, int pageSize, out int totalCount)
        {
            using (var dbContext = new MallDbContext())
            {
                var query = dbContext.Goods.Where(me => me.IsPresell && me.Status == GoodsStatus.InSale)
                    .OrderByDescending(me => me.Sort).ThenByDescending(me => me.CreateTime);
                totalCount = query.Count();
                return query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
        }

        /// <summary>
        /// 团购列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<Goods> GetGroupGoods(int pageIndex, int pageSize, out int totalCount)
        {
            using (var dbContext = new MallDbContext())
            {
                var query = dbContext.Goods.Where(me => me.IsGroupon && me.Status == GoodsStatus.InSale)
                    .OrderByDescending(me => me.Sort).ThenByDescending(me => me.CreateTime);
                totalCount = query.Count();
                return query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
        }

        /// <summary>
        /// 获取商品所包含的属性和属性值
        /// </summary>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        public List<GoodsAttiuteValues> GetGoodsAttributeValues(Guid goodsId)
        {
            using (var dbContext = new MallDbContext())
            {
                var query = from gar in dbContext.SingleGoodsAttributes
                            join gs in dbContext.SingleGoods on gar.SingleGoodsId equals gs.Id
                            join ga in dbContext.GoodsAttributes on gar.AttributeId equals ga.Id
                            where gs.GoodsId == goodsId
                            group new { gar, ga } by new { gar.AttributeId, ga.Name }
                    into g
                            select
                                new
                                {
                                    AttributeId = g.Key.AttributeId,
                                    AttributeName = g.Key.Name,
                                    AttributeValues = g.Select(gar => gar.gar.AttributeValue)
                                };
                var data = query.OrderBy(a => a.AttributeId).ToList();

                var resData = data.Select(item => new GoodsAttiuteValues()
                {
                    AttributeId = item.AttributeId,
                    AttributeName = item.AttributeName,
                    AttributeValues = string.Join(",", item.AttributeValues.Distinct().ToList())
                }).ToList();

                return resData;
            };
        }


        #region 商品收藏
        public List<Models.Goods> LoadCollectGoodsByPage(string memberId, int pageIndex, int pageSize, out int totalCount)
        {
            using (var dbContext = new MallDbContext())
            {
                var query = (from m in dbContext.Markups
                             join goods in dbContext.Goods
                                 on m.SourceId equals goods.Id
                             where
                                 m.MemberId == memberId && m.ModuleKey.Equals(MallModule.Key, StringComparison.OrdinalIgnoreCase) &&
                                 m.MarkupType == MarkupType.Collect && goods.Status != GoodsStatus.Delete
                             select goods).OrderByDescending(x => x.CreateTime);

                totalCount = query.Count();
                return query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
        }

        #endregion

        #region 商品浏览
        public List<BrowseViewModel> LoadBrowseGoodsByPage(string memberId, int pageIndex, int pageSize, out int totalCount)
        {
            using (var dbContext = new MallDbContext())
            {
                var query = (from m in dbContext.Markups
                             join goods in dbContext.Goods
                                 on m.SourceId equals goods.Id
                             where
                                 m.MemberId == memberId && m.ModuleKey.Equals(MallModule.Key, StringComparison.OrdinalIgnoreCase) &&
                                 m.MarkupType == MarkupType.Browse && goods.Status != GoodsStatus.Delete
                             select new
                             {
                                 m.CreateTime,
                                 goods.Id,
                                 goods.Name,
                                 goods.ShopPrice,
                                 goods.Status
                             }).OrderByDescending(x => x.CreateTime);

                totalCount = query.Count();

                var data = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

                var resData = data.Select(item => new BrowseViewModel()
                {
                    Id = item.Id,
                    Name = item.Name,
                    ShopPrice = item.ShopPrice,
                    Status = item.Status,
                    BrowseTime = item.CreateTime
                }).ToList();

                return resData;
            }
        }
        #endregion

        #region 单品

        public bool CheckSingleGoodsIsInvalid(Guid singleGoodsId, decimal originalPrice)
        {
            using (var dbContext = new MallDbContext())
            {
                var singleGoods = dbContext.SingleGoods.Include(sg => sg.Goods).FirstOrDefault(g => g.Id.Equals(singleGoodsId));
                if (singleGoods != null && singleGoods.Goods.Status == GoodsStatus.InSale && singleGoods.Stock > 0
                    )
                {
                    if (singleGoods.Goods.IsGroupon && singleGoods.Goods.GrouponStartTime <= DateTime.Now &&
                        singleGoods.Goods.GrouponEndTime >= DateTime.Now && singleGoods.GrouponPrice == originalPrice)
                    {
                        return false;
                    }
                    else if (singleGoods.Price == originalPrice)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        #endregion

        #region 商品评价

        public List<Evaluate.Models.Evaluate> GetGoodsEvaluatesListByPage(Guid goodsId, int pageIndex, int pageSize, out int totalCount)
        {
            using (var dbContext = new MallDbContext())
            {
                var query = (from e in dbContext.Evaluates
                             join s in dbContext.SingleGoods
                                 on e.ExtentsionId equals s.Id
                             where
                                 e.SourceType.Equals("Order") && e.ModuleKey.Equals(OrderProcessModule.Key, StringComparison.OrdinalIgnoreCase) &&
                                s.GoodsId == goodsId
                             select e).OrderByDescending(x => x.CreateTime);

                totalCount = query.Count();

                return query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
        }

        public List<Evaluate.Models.Evaluate> GetNew9Evaluates(Guid goodsId)
        {
            using (var dbContext = new MallDbContext())
            {
                var query = (from e in dbContext.Evaluates
                             join s in dbContext.SingleGoods
                                 on e.ExtentsionId equals s.Id
                             where
                                 e.SourceType.Equals("Order") && e.ModuleKey.Equals(OrderProcessModule.Key, StringComparison.OrdinalIgnoreCase) &&
                                s.GoodsId == goodsId
                             select e).DistinctBy(x => x.MemberId).OrderByDescending(x => x.CreateTime);

                return query.Take(9).ToList();
            }
        }
        #endregion

        /// <summary>
        /// 根据条件获取所有商品信息
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public List<Goods> GetList(Expression<Func<Goods, bool>> expression)
        {
            using (var dbContex = new MallDbContext())
            {
                var query = dbContex.Goods.Where(expression).ToList();
                return query;
            }
        }
    }
}