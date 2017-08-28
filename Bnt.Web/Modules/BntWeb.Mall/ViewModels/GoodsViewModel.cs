/* 
    ======================================================================== 
        File name：        GoodsViewModel
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/7/5 15:18:59
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BntWeb.Mall.Models;

namespace BntWeb.Mall.ViewModels
{
    public class GoodsViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public Guid? BrandId { get; set; }

        public Guid CategoryId { get; set; }

        public string GoodsNo { get; set; }

        public decimal OriginalPrice { get; set; }

        public string Description { get; set; }

        public Guid GoodsType { get; set; }

        public string SingleGoodsJson { get; set; }

        public List<SingleGoodsViewModel> SingleGoods { get; set; }

        /// <summary>
        /// 扩展分类Ids，英文逗号分隔
        /// </summary>
        public string ExtendCategoryIds { set; get; }
        /// <summary>
        /// 主图
        /// </summary>
        public string MainImage { set; get; }

        public bool IsNew { set; get; }

        public bool IsBest { set; get; }

        public bool IsHot { set; get; }

        public bool FreeShipping { set; get; }

        public decimal UsualWeight { get; set; }

        public decimal[] Commission { get; set; }

        public bool IsPresell { get; set; }

        public string Brief { get; set; }

        public int Sort { get; set; }
    }

    public class SingleGoodsViewModel
    {
        public Guid Id { get; set; }

        public int Stock { get; set; }

        public string Unit { get; set; }

        public decimal Price { get; set; }

        public List<AttrViewModel> Attrs { get; set; }

        public SingleGoodsImage Image { get; set; }

        public decimal Weight { get; set; }

        public decimal GrouponPrice { get; set; }
    }
    public class SingleGoodsImage
    {
        public Guid Id { get; set; }

        public string RelativePath { get; set; }
        /// <summary>
        /// 中等尺寸缩略图，图片类型可用
        /// 
        /// </summary>
        public string MediumThumbnail { get; set; }
        /// <summary>
        /// 小尺寸缩略图，图片类型可用
        /// 
        /// </summary>
        public string SmallThumbnail { get; set; }
    }

    public class AttrViewModel
    {
        public Guid Id { get; set; }

        public string Val { get; set; }
    }
    public class SelectedAttrViewModel
    {
        public Guid Id { get; set; }

        public List<string> Vals { get; set; }
    }

    /// <summary>
    /// 商品浏览记录模型
    /// </summary>
    public class BrowseViewModel
    {
        public Guid Id { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 售价，如果有多个规格的商品，取最低价格
        /// </summary>
        public decimal ShopPrice { get; set; }

        /// <summary>
        /// 商品状态 0-失效 1-正常
        /// </summary>
        public Models.GoodsStatus Status { get; set; }

        /// <summary>
        /// 浏览时间
        /// </summary>
        public DateTime BrowseTime { get; set; }
    }

    public class ExtendGoodsViewModelnew
    {
        public GoodsView GoodsView { get; set; }
        /// <summary>
        /// 扩展属性
        /// </summary>
        public Guid? ExtendCategoryId { get; set; }

        public string MergerCategoryId { set; get; }
    }
}