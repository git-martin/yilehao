using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BntWeb.Mall.ViewModels
{
    #region 商品属性相关
    /// <summary>
    /// 商品属性-值对象
    /// </summary>
    public class GoodsAttiuteValues
    {

        /// <summary>
        /// 属性ID
        /// </summary>
        public Guid AttributeId { set; get; }
        /// <summary>
        /// 属性名称
        /// </summary>
        public string AttributeName { set; get; }
        /// <summary>
        /// 多个属性值，格式：值1,值2,值3
        /// 每个值之间用英文,逗号隔开
        /// </summary>
        public string AttributeValues { set; get; }
    }
    
    /// <summary>
    /// 单品属性分组
    /// </summary>
    public class SingleGoodsAttrGroup
    {
        /// <summary>
        /// 商品Id
        /// </summary>
        public Guid GoodsId { set; get; }
        /// <summary>
        /// 单品ID
        /// </summary>
        public Guid SingleGoodsId { set; get; }
        /// <summary>
        /// 单品货号
        /// </summary>
        public string SingleGoodsNo { set; get; }
        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { set; get; }
        /// <summary>
        /// 库存
        /// </summary>
        public int Stock { set; get; }
        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { set; get; }
        /// <summary>
        /// 属性列表
        /// </summary>
        public List<GoodsAttiuteValues> SingleGoodsAttributeList { set; get; }
    }
    
    #endregion
}
