using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using BntWeb.Environment;
using BntWeb.FileSystems.Media;
using BntWeb.Mall.Models;

namespace BntWeb.Mall.ApiModels
{
    /// <summary>
    /// 树形结构分类
    /// </summary>
    public class GoodsCategoryModel
    {
        /// <summary>
		/// 
		/// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 分类图片
        /// </summary>
        public SimplifiedStorageFile CategoryImage { get; set; }

        public List<GoodsCategoryModel> ChildGoodsCategorys { set; get; }
    }

    /// <summary>
    /// 指定固定分类
    /// </summary>
    public class HomeGoodsCategoryModel
    {
        /// <summary>
		/// 
		/// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 商品
        /// </summary>
        public List<ListGoodsModel> Goods = new List<ListGoodsModel>();
    }

    /// <summary>
    /// 一级分类
    /// </summary>
    public class OneLevelCategoryModel
    {
        /// <summary>
		/// 
		/// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 分类图片
        /// </summary>
        public SimplifiedStorageFile CategoryImage { get; set; }

        public OneLevelCategoryModel(GoodsCategory model)
        {
            Id = model.Id;
            Name = model.Name;
            var fileService = HostConstObject.Container.Resolve<IStorageFileService>();
            var mainImage = fileService.GetFiles(model.Id, MallModule.Key, "CategoryImage").FirstOrDefault();
            CategoryImage = mainImage?.Simplified();
        }
    }
}
