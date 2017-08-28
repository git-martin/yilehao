using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using BntWeb.Environment;
using BntWeb.FileSystems.Media;
using BntWeb.Mall.Models;
using BntWeb.Mall.Services;

namespace BntWeb.Mall.ApiModels
{
    public class CreateCartModel
    {
        /// <summary>
        /// 商品Id
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 单品Id
        /// </summary>
        public Guid SingleGoodsId { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }
        
    }

    public class EditCartModel
    {
        /// <summary>
        /// 商品Id
        /// </summary>
        public Guid CartId { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }

    }

    public class ListCartModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 商品Id
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 单品Id
        /// </summary>
        public Guid SingleGoodsId { get; set; }

        /// <summary>
        /// 属性组合值
        /// </summary>
        public string GoodsAttribute { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }
        

        /// <summary>
        /// 
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 状态：1正常，0失效（商品信息修改后，购物车相关的商品设置为失效）
        /// </summary>
        public CartStatus Status { get; set; }

        /// <summary>
        /// 是否免邮
        /// </summary>
        public bool FreeShipping { get; set; }

        public SimplifiedStorageFile GoodsImage { set; get; }

        public ListCartModel(Cart model)
        {
            Id = model.Id;
            GoodsId = model.GoodsId;
            SingleGoodsId = model.SingleGoodsId;
            GoodsAttribute = model.GoodsAttribute;
            Quantity = model.Quantity;
            GoodsName = model.GoodsName;
            Price = model.Price;
            FreeShipping = model.FreeShipping;
            var goodsService = HostConstObject.Container.Resolve<IGoodsService>();
            bool isInvalid=goodsService.CheckSingleGoodsIsInvalid(model.SingleGoodsId, model.Price);
            Status = isInvalid ? CartStatus.Invalid : CartStatus.Normal;
            var fileService = HostConstObject.Container.Resolve<IStorageFileService>();
            var mainImage = fileService.GetFiles(model.GoodsId, MallModule.Key, "MainImage").FirstOrDefault();
            GoodsImage = mainImage?.Simplified();
        }
    }
}
