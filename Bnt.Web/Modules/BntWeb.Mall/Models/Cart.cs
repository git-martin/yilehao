/* 
    ======================================================================== 
        File name：		Cart
        Module:			
        Author：		Daniel.Wu（wujb）
        Create Time：		2016/7/6 10:57:50
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using BntWeb.Data;

namespace BntWeb.Mall.Models
{
    /// <summary>
    /// 实体：Carts
    /// </summary>
    [Table(KeyGenerator.TablePrefix + "Carts")]
    public class Cart
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string MemberId { get; set; }

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

        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public string GoodsNo { get; set; }

        /// <summary>
        /// 属性组合值
        /// </summary>
        public string GoodsAttribute { get; set; }

        /// <summary>
        /// 单品编号
        /// </summary>
        public string SingleGoodsNo { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 状态：1正常，0失效（商品信息修改后，购物车相关的商品设置为失效）
        /// </summary>
        public CartStatus Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 是否免邮
        /// </summary>
        public bool FreeShipping { get; set; }

    }

    public enum CartStatus
    {
        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        Normal = 1,
        /// <summary>
        /// 无效
        /// </summary>
        [Description("无效")]
        Invalid = 0,
    }
}