using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using BntWeb.Environment;
using BntWeb.FileSystems.Media;

namespace BntWeb.Mall.ApiModels
{
    public class CollectListModel
    {
        public Guid Id { get; set; }

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
        /// 付款数量
        /// </summary>
        public int PaymentAmount { get; set; }

        /// <summary>
        /// 销量
        /// </summary>
        public int SalesVolume { get; set; }

        /// <summary>
        /// 主图
        /// </summary>
        public SimplifiedStorageFile MainImage { set; get; }

        /// <summary>
        /// 商品状态 0-失效 1-正常
        /// </summary>
        public int Status { get; set; }

        public CollectListModel(Models.Goods model)
        {
            Id = model.Id;
            Name = model.Name;
            OriginalPrice = model.OriginalPrice;
            ShopPrice = model.ShopPrice;
            SalesVolume = model.SalesVolume;
            PaymentAmount = model.PaymentAmount;
            Status = model.Status== Models.GoodsStatus.InSale?1:0;
            var fileService = HostConstObject.Container.Resolve<IStorageFileService>();
            var mainImage = fileService.GetFiles(model.Id, MallModule.Key, "MainImage").FirstOrDefault();
            MainImage = mainImage?.Simplified();
        }
    }
}