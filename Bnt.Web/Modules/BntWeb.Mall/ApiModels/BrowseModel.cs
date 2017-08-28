using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using BntWeb.Environment;
using BntWeb.FileSystems.Media;

namespace BntWeb.Mall.ApiModels
{
    public class BrowseListModel
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
        /// 主图
        /// </summary>
        public SimplifiedStorageFile MainImage { set; get; }

        /// <summary>
        /// 商品状态 0-失效 1-正常
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 浏览时间
        /// </summary>
        public DateTime BrowseTime { get; set; }

        public BrowseListModel(ViewModels.BrowseViewModel model)
        {
            Id = model.Id;
            Name = model.Name;
            ShopPrice = model.ShopPrice;
            Status = model.Status == Models.GoodsStatus.InSale ? 1 : 0;
            var fileService = HostConstObject.Container.Resolve<IStorageFileService>();
            var mainImage = fileService.GetFiles(model.Id, MallModule.Key, "MainImage").FirstOrDefault();
            MainImage = mainImage?.Simplified();
            BrowseTime = model.BrowseTime;
        }
    }
}