/* 
    ======================================================================== 
        File name：        Goods
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/7/1 11:37:58
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Autofac;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.Environment;
using BntWeb.FileSystems.Media;

namespace BntWeb.Mall.Models
{
    [Table(KeyGenerator.TablePrefix + "Goods")]
    public class Goods
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 主分类
        /// </summary>
        public Guid CategoryId { set; get; }

        /// <summary>
        /// 品牌
        /// </summary>
        public Guid? BrandId { set; get; }

        /// <summary>
        /// 商品类型
        /// </summary>
        public Guid GoodsType { set; get; }

        /// <summary>
        /// 货号
        /// </summary>
        [MaxLength(50)]
        public string GoodsNo { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [MaxLength(200)]
        public string Name { get; set; }

        /// <summary>
        /// 原价
        /// </summary>
        public decimal OriginalPrice { get; set; }

        /// <summary>
        /// 售价，如果有多个规格的商品，取最低价格
        /// </summary>
        public decimal ShopPrice { get; set; }

        /// <summary>
        /// 商品描述
        /// </summary>
        [MaxLength(600000)]
        public string Description { get; set; }

        /// <summary>
        /// 是否新品
        /// </summary>
        public bool IsNew { get; set; }

        /// <summary>
        /// 是否精品
        /// </summary>
        public bool IsBest { get; set; }

        /// <summary>
        /// 是否热卖
        /// </summary>
        public bool IsHot { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime LastUpdateTime { get; set; }

        /// <summary>
        /// 库存
        /// </summary>
        public int Stock { get; set; }

        /// <summary>
        /// 付款数量
        /// </summary>
        public int PaymentAmount { get; set; }

        /// <summary>
        /// 销量
        /// </summary>
        public int SalesVolume { get; set; }

        /// <summary>
        /// 评价数
        /// </summary>
        public int EvaluateCount { get; set; }

        /// <summary>
        /// 查看次数
        /// </summary>
        public int ViewCount { get; set; }

        /// <summary>
        /// 商品状态
        /// </summary>
        public GoodsStatus Status { get; set; }

        /// <summary>
        /// 排序，从大到小
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 是否免邮
        /// </summary>
        public bool FreeShipping { get; set; }

        /// <summary>
        /// 商品重量
        /// </summary>
        public decimal UsualWeight { get; set; }

        /// <summary>
        /// 团购价，如果有多个规格的商品，取最低价格
        /// </summary>
        public decimal GrouponPrice { get; set; }

        /// <summary>
        /// 是否参加团购
        /// </summary>
        public bool IsGroupon { get; set; }

        /// <summary>
        /// 团购开始时间
        /// </summary>
        public DateTime GrouponStartTime { get; set; }

        /// <summary>
        /// 团购结束时间
        /// </summary>
        public DateTime GrouponEndTime { get; set; }

        [NotMapped]
        public virtual List<SingleGoods> SingleGoods { get; set; }
        /// <summary>
        /// 是否预售
        /// </summary>
        public bool IsPresell { get; set; }
        /// <summary>
        /// 简述
        /// </summary>
        [MaxLength(500)]
        public string Brief { get; set; }

    }

    public enum GoodsStatus
    {
        /// <summary>
        /// 已删除
        /// </summary>
        [Description("已删除")]
        Delete = -1,

        /// <summary>
        /// 未上架
        /// </summary>
        [Description("未上架")]
        NotInSale = 0,

        /// <summary>
        /// 在售
        /// </summary>
        [Description("在售")]
        InSale = 1
    }

    public enum GoodsSortType
    {

        /// <summary>
        /// 默认排序
        /// </summary>
        [Description("默认排序")]
        Degault = 0,
        /// <summary>
        /// 价格从低到高
        /// </summary>
        [Description("价格从低到高")]
        PriceLow = 1,
        /// <summary>
        /// 价格从高到低
        /// </summary>
        [Description("价格从高到低")]
        PriceHigh = 2,
        /// <summary>
        /// 销量优先
        /// </summary>
        [Description("销量优先")]
        SalesVolumeHigh
    }

    [Table(KeyGenerator.TablePrefix + "View_Goods")]
    public class GoodsView
    {
        public Guid Id { get; set; }

        /// <summary>
        /// 主分类
        /// </summary>
        public Guid CategoryId { set; get; }

        /// <summary>
        /// 品牌
        /// </summary>
        public Guid? BrandId { set; get; }

        /// <summary>
        /// 商品类型
        /// </summary>
        public Guid GoodsType { set; get; }

        /// <summary>
        /// 货号
        /// </summary>
        public string GoodsNo { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 原价
        /// </summary>
        public decimal OriginalPrice { get; set; }

        /// <summary>
        /// 售价，如果有多个规格的商品，取最低价格
        /// </summary>
        public decimal ShopPrice { get; set; }

        /// <summary>
        /// 商品描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 是否新品
        /// </summary>
        public bool IsNew { get; set; }

        /// <summary>
        /// 是否精品
        /// </summary>
        public bool IsBest { get; set; }

        /// <summary>
        /// 是否热卖
        /// </summary>
        public bool IsHot { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime LastUpdateTime { get; set; }

        /// <summary>
        /// 库存
        /// </summary>
        public int Stock { get; set; }

        /// <summary>
        /// 付款数量
        /// </summary>
        public int PaymentAmount { get; set; }

        /// <summary>
        /// 销量
        /// </summary>
        public int SalesVolume { get; set; }

        /// <summary>
        /// 评价数
        /// </summary>
        public int EvaluateCount { get; set; }

        /// <summary>
        /// 查看次数
        /// </summary>
        public int ViewCount { get; set; }

        /// <summary>
        /// 商品状态
        /// </summary>
        public GoodsStatus Status { get; set; }

        /// <summary>
        /// 排序，从大到小
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 是否免邮
        /// </summary>
        public bool FreeShipping { get; set; }

        /// <summary>
        /// 单品Id
        /// </summary>
        public Guid SingleGoodsId { get; set; }

        /// <summary>
        /// 属性Id
        /// </summary>
        public Guid AttributeId { get; set; }

        /// <summary>
        /// 属性值 多个用,分隔
        /// </summary>
        public string AttributeValue { get; set; }

        /// <summary>
        /// 团购价，如果有多个规格的商品，取最低价格
        /// </summary>
        public decimal GrouponPrice { get; set; }

        /// <summary>
        /// 是否参加团购
        /// </summary>
        public bool IsGroupon { get; set; }

        /// <summary>
        /// 团购开始时间
        /// </summary>
        public DateTime GrouponStartTime { get; set; }

        /// <summary>
        /// 团购结束时间
        /// </summary>
        public DateTime GrouponEndTime { get; set; }

        public bool IsPresell { get; set; }

    }

    /// <summary>
    /// 商品信息和分类查询视图
    /// </summary>
    [Table(KeyGenerator.TablePrefix + "View_Goods_Category")]
    public class GoodsCategoryView
    {
        public Guid Id { get; set; }

        /// <summary>
        /// 主分类
        /// </summary>
        public Guid CategoryId { set; get; }

        /// <summary>
        /// 品牌
        /// </summary>
        public Guid? BrandId { set; get; }

        /// <summary>
        /// 商品类型
        /// </summary>
        public Guid GoodsType { set; get; }

        /// <summary>
        /// 货号
        /// </summary>
        public string GoodsNo { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 原价
        /// </summary>
        public decimal OriginalPrice { get; set; }

        /// <summary>
        /// 售价，如果有多个规格的商品，取最低价格
        /// </summary>
        public decimal ShopPrice { get; set; }

        /// <summary>
        /// 是否新品
        /// </summary>
        public bool IsNew { get; set; }

        /// <summary>
        /// 是否精品
        /// </summary>
        public bool IsBest { get; set; }

        /// <summary>
        /// 是否热卖
        /// </summary>
        public bool IsHot { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime LastUpdateTime { get; set; }

        /// <summary>
        /// 库存
        /// </summary>
        public int Stock { get; set; }

        /// <summary>
        /// 付款数量
        /// </summary>
        public int PaymentAmount { get; set; }

        /// <summary>
        /// 销量
        /// </summary>
        public int SalesVolume { get; set; }

        /// <summary>
        /// 评价数
        /// </summary>
        public int EvaluateCount { get; set; }

        /// <summary>
        /// 查看次数
        /// </summary>
        public int ViewCount { get; set; }

        /// <summary>
        /// 商品状态
        /// </summary>
        public GoodsStatus Status { get; set; }

        /// <summary>
        /// 排序，从大到小
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 是否免邮
        /// </summary>
        public bool FreeShipping { get; set; }

        /// <summary>
        /// 扩展分类Id
        /// </summary>
        public Guid ExtentCategoryId { get; set; }

        /// <summary>
        /// 分类组合ID
        /// </summary>
        public string MergerId { get; set; }

    }

    [Table(KeyGenerator.TablePrefix + "View_Goods_Category_Name")]
    public class GoodsCategoryNameView
    {
        public Guid Id { get; set; }


        public string GoodsNo { get; set; }


        public string Name { get; set; }

        /// <summary>
        /// 原价
        /// </summary>
        public decimal OriginalPrice { get; set; }

        /// <summary>
        /// 售价，如果有多个规格的商品，取最低价格
        /// </summary>
        public decimal ShopPrice { get; set; }

        /// <summary>
        /// 是否新品
        /// </summary>
        public bool IsNew { get; set; }

        /// <summary>
        /// 是否精品
        /// </summary>
        public bool IsBest { get; set; }

        /// <summary>
        /// 是否热卖
        /// </summary>
        public bool IsHot { get; set; }

        /// <summary>
        /// 销量
        /// </summary>
        public int SalesVolume { get; set; }

        /// <summary>
        /// 商品状态
        /// </summary>
        public GoodsStatus Status { get; set; }

        /// <summary>
        /// 库存
        /// </summary>
        public int Stock { get; set; }

        public Guid CategoryId { set; get; }

        [MaxLength(100)]
        public string CategoryName { set; get; }

        /// <summary>
        /// 合并id
        /// </summary>
        [MaxLength(500)]
        public string MergerId { get; set; }

        public DateTime CreateTime { get; set; }

        public bool IsPresell { get; set; }
    }

    /// <summary>
    /// 单品商品信息和分类组合查询视图
    /// </summary>
    [Table(KeyGenerator.TablePrefix + "View_Single_Goods_Category")]
    public class SingleGoodsCategoryView
    {
        public Guid Id { get; set; }

        /// <summary>
        /// 主分类
        /// </summary>
        public Guid CategoryId { set; get; }

        /// <summary>
        /// 品牌
        /// </summary>
        public Guid? BrandId { set; get; }

        /// <summary>
        /// 商品类型
        /// </summary>
        public Guid GoodsType { set; get; }

        /// <summary>
        /// 货号
        /// </summary>
        public string GoodsNo { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 原价
        /// </summary>
        public decimal OriginalPrice { get; set; }

        /// <summary>
        /// 售价，如果有多个规格的商品，取最低价格
        /// </summary>
        public decimal ShopPrice { get; set; }


        /// <summary>
        /// 是否新品
        /// </summary>
        public bool IsNew { get; set; }

        /// <summary>
        /// 是否精品
        /// </summary>
        public bool IsBest { get; set; }

        /// <summary>
        /// 是否热卖
        /// </summary>
        public bool IsHot { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime LastUpdateTime { get; set; }

        /// <summary>
        /// 库存
        /// </summary>
        public int Stock { get; set; }

        /// <summary>
        /// 付款数量
        /// </summary>
        public int PaymentAmount { get; set; }

        /// <summary>
        /// 销量
        /// </summary>
        public int SalesVolume { get; set; }

        /// <summary>
        /// 评价数
        /// </summary>
        public int EvaluateCount { get; set; }

        /// <summary>
        /// 查看次数
        /// </summary>
        public int ViewCount { get; set; }

        /// <summary>
        /// 商品状态
        /// </summary>
        public GoodsStatus Status { get; set; }

        /// <summary>
        /// 排序，从大到小
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 是否免邮
        /// </summary>
        public bool FreeShipping { get; set; }

        /// <summary>
        /// 单品Id
        /// </summary>
        public Guid SingleGoodsId { get; set; }

        /// <summary>
        /// 属性Id
        /// </summary>
        public Guid AttributeId { get; set; }

        /// <summary>
        /// 属性值 多个用,分隔
        /// </summary>
        public string AttributeValue { get; set; }
        /// <summary>
        /// 扩展分类Id
        /// </summary>
        public Guid ExtentCategoryId { get; set; }

        /// <summary>
        /// 分类组合ID
        /// </summary>
        public string MergerId { get; set; }

    }


    public class ListShortGoodsModel
    {

        public Guid Id { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 货号
        /// </summary>
        [MaxLength(50)]
        public string GoodsNo { get; set; }

        public decimal OriginalPrice { get; set; }

        /// <summary>
        /// 售价，如果有多个规格的商品，取最低价格
        /// </summary>
        public decimal ShopPrice { get; set; }

        /// <summary>
        /// 是否新品
        /// </summary>
        public bool IsNew { get; set; }

        /// <summary>
        /// 是否精品
        /// </summary>
        public bool IsBest { get; set; }

        /// <summary>
        /// 是否热卖
        /// </summary>
        public bool IsHot { get; set; }

        /// <summary>
        /// 付款数量
        /// </summary>
        public int PaymentAmount { get; set; }

        /// <summary>
        /// 销量
        /// </summary>
        public int SalesVolume { get; set; }

        /// <summary>
        /// 商品状态
        /// </summary>
        public GoodsStatus Status { get; set; }

        /// <summary>
        /// 库存
        /// </summary>
        public int Stock { get; set; }
        /// <summary>
        /// 团购价，如果有多个规格的商品，取最低价格
        /// </summary>
        public decimal GrouponPrice { get; set; }

        /// <summary>
        /// 是否参加团购
        /// </summary>
        public bool IsGroupon { get; set; }

        /// <summary>
        /// 团购开始时间
        /// </summary>
        public DateTime GrouponStartTime { get; set; }

        /// <summary>
        /// 团购结束时间
        /// </summary>
        public DateTime GrouponEndTime { get; set; }
        /// <summary>
        /// 主图
        /// </summary>
        public SimplifiedStorageFile MainImage { set; get; }

        public ListShortGoodsModel(Goods model)
        {
            Id = model.Id;
            Name = model.Name;
            GoodsNo = model.GoodsNo;
            OriginalPrice = model.OriginalPrice;
            ShopPrice = model.ShopPrice;
            IsNew = model.IsNew;
            IsHot = model.IsHot;
            IsBest = model.IsBest;
            SalesVolume = model.SalesVolume;
            PaymentAmount = model.PaymentAmount;
            Status = model.Status;
            Stock = model.Stock;
            IsGroupon = model.IsGroupon;
            GrouponStartTime = model.GrouponStartTime;
            GrouponEndTime = model.GrouponEndTime;
            var fileService = HostConstObject.Container.Resolve<IStorageFileService>();
            var mainImage = fileService.GetFiles(model.Id, MallModule.Key, "MainImage").FirstOrDefault();
            MainImage = mainImage?.Simplified();
        }
    }


    public class ListGoodsCategoryViewModel
    {

        public Guid Id { get; set; }


        public string GoodsNo { get; set; }


        public string Name { get; set; }

        /// <summary>
        /// 原价
        /// </summary>
        public decimal OriginalPrice { get; set; }

        /// <summary>
        /// 售价，如果有多个规格的商品，取最低价格
        /// </summary>
        public decimal ShopPrice { get; set; }

        /// <summary>
        /// 是否新品
        /// </summary>
        public bool IsNew { get; set; }

        /// <summary>
        /// 是否精品
        /// </summary>
        public bool IsBest { get; set; }

        /// <summary>
        /// 是否热卖
        /// </summary>
        public bool IsHot { get; set; }

        /// <summary>
        /// 商品状态
        /// </summary>
        public GoodsStatus Status { get; set; }

        /// <summary>
        /// 库存
        /// </summary>
        public int Stock { get; set; }

        public Guid CategoryId { set; get; }

        [MaxLength(100)]
        public string CategoryName { set; get; }

        /// <summary>
        /// 合并id
        /// </summary>
        [MaxLength(500)]
        public string MergerId { get; set; }
        /// <summary>
        /// 主图
        /// </summary>
        public SimplifiedStorageFile MainImage { set; get; }

        public bool IsPreSell { get; set;  }

        public ListGoodsCategoryViewModel(GoodsCategoryNameView model)
        {
            Id = model.Id;
            Name = model.Name;
            GoodsNo = model.GoodsNo;
            OriginalPrice = model.OriginalPrice;
            ShopPrice = model.ShopPrice;
            IsNew = model.IsNew;
            IsHot = model.IsHot;
            IsBest = model.IsBest;
            Status = model.Status;
            Stock = model.Stock;
            CategoryId = model.CategoryId;
            CategoryName = model.CategoryName;
            MergerId = model.MergerId;
            var fileService = HostConstObject.Container.Resolve<IStorageFileService>();
            var mainImage = fileService.GetFiles(model.Id, MallModule.Key, "MainImage").FirstOrDefault();
            MainImage = mainImage?.Simplified();
            IsPreSell = model.IsPresell;
        }
    }
}