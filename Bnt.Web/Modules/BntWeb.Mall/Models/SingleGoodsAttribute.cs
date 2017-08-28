/* 
    ======================================================================== 
        File name：        SingleGoodsAttribute
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/7/1 11:38:32
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using BntWeb.Data;

namespace BntWeb.Mall.Models
{
    [Table(KeyGenerator.TablePrefix + "Goods_Attribute_Ralations")]
    public class SingleGoodsAttribute
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 单品Id
        /// </summary>
        public Guid SingleGoodsId { get; set; }

        /// <summary>
        /// 属性Id
        /// </summary>
        public Guid AttributeId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [MaxLength(200)]
        public string AttributeValue { get; set; }

        ///// <summary>
        ///// 所属单品
        ///// </summary>
        //[ForeignKey("SingleGoodsId")]
        //public virtual SingleGoods SingleGoods { get; set; }
        
        ///// <summary>
        ///// 关联属性
        ///// </summary>
        //[ForeignKey("AttributeId")]
        //public virtual GoodsAttribute GoodsAttribute { get; set; }
    }
}