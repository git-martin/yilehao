using System;
using System.Linq;
using Autofac;
using BntWeb.Environment;
using BntWeb.FileSystems.Media;
using BntWeb.Mall;

namespace BntWeb.Discovery.ApiModels
{
    public class DiscoveryListModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Blurb { get; set; }

        public DateTime CreateTime { get; set; }

        public int ReadNum { get; set; }

        /// <summary>
        /// 主图
        /// </summary>
        public SimplifiedStorageFile MainImage { set; get; }


        public DiscoveryListModel(Models.Discovery model)
        {
            Id = model.Id;
            Title = model.Title;
            Blurb = model.Blurb;
            CreateTime = model.CreateTime;
            ReadNum = model.ReadNum;
            var fileService = HostConstObject.Container.Resolve<IStorageFileService>();
            var mainImage = fileService.GetFiles(model.Id, DiscoveryModule.Key, "DiscoveryImages").FirstOrDefault();
            MainImage = mainImage?.Simplified();
        }
    }


    public class DiscoveryModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime CreateTime { get; set; }

        public int ReadNum { get; set; }

        /// <summary>
        /// 主图
        /// </summary>
        public SimplifiedStorageFile MainImage { set; get; }


        public DiscoveryModel(Models.Discovery model)
        {
            Id = model.Id;
            Title = model.Title;
            Content = model.Content;
            CreateTime = model.CreateTime;
            ReadNum = model.ReadNum;
            var fileService = HostConstObject.Container.Resolve<IStorageFileService>();
            var mainImage = fileService.GetFiles(model.Id, DiscoveryModule.Key, "DiscoveryImages").FirstOrDefault();
            MainImage = mainImage?.Simplified();
        }
    }

    public class DiscoveryRelationGoodsModel
    {
        public Guid GoodsId { get; set; }

        public string GoodsName { get; set; }

        public decimal OriginalPrice { get; set; }

        public int PaymentAmount { get; set; }
        
        /// <summary>
        /// 主图
        /// </summary>
        public SimplifiedStorageFile MainImage { set; get; }


        public DiscoveryRelationGoodsModel(Models.DiscoveryGoodsRelation model)
        {
            GoodsId = model.Goods.Id;
            GoodsName = model.Goods.Name;
            OriginalPrice = model.Goods.OriginalPrice;
            PaymentAmount = model.Goods.PaymentAmount;

            var fileService = HostConstObject.Container.Resolve<IStorageFileService>();
            var mainImage = fileService.GetFiles(model.Id, MallModule.Key, "MainImage").FirstOrDefault();
            MainImage = mainImage?.Simplified();
        }
    }
}