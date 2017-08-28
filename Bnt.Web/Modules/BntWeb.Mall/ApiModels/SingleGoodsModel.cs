using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BntWeb.FileSystems.Media;
using BntWeb.Mall.Models;

namespace BntWeb.Mall.ApiModels
{
    public class SingleGoodsModel
    {
        public Guid SingleGoodsId { get; set; }

        /// <summary>
        /// 货号
        /// </summary>
        public string SingleGoodsNo { get; set; }

        /// <summary>
        /// 售价
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 库存
        /// </summary>
        public int Stock { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        public SimplifiedStorageFile Image { get; set; }

        public decimal GrouponPrice { get; set; }

        /// <summary>
        /// 排序，从大到小
        /// </summary>
        public int Sort { get; set; }

        public List<SingleGoodsAttributeModel> Attributes { get; set; }

        public SingleGoodsModel(SingleGoods model)
        {
            SingleGoodsId = model.Id;
            SingleGoodsNo = model.SingleGoodsNo;
            Price = model.Price;
            Stock = model.Stock;
            Unit = model.Unit;
            Image = model.Image;
            GrouponPrice=model.GrouponPrice;

            Attributes = model.Attributes.Select(me => new SingleGoodsAttributeModel
            {
                AttributeId = me.AttributeId,
                AttributeValue = me.AttributeValue
            }).ToList();
        }
    }

    public class SingleGoodsAttributeModel
    {
        /// <summary>
        /// 属性Id
        /// </summary>
        public Guid AttributeId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AttributeValue { get; set; }
    }
}
