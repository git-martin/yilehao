
/* Models Code Auto-Generation 
    ======================================================================== 
        File name：		OrderGoods
        Module:			
        Author：		罗嗣宝
        Create Time：		2016/7/6 16:49:27
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System; 
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using BntWeb.Data;
using BntWeb.FileSystems.Media;

namespace BntWeb.OrderProcess.Models
{    
    /// <summary>
    /// 实体：OrderGoods
    /// </summary>
    [Table(KeyGenerator.TablePrefix + "Order_Goods")]
    public class OrderGoods
    {
        /// <summary>
        /// 
        /// </summary>
        [Key]
        public Guid Id { get; set;}
        
        /// <summary>
		/// 
		/// </summary>
        public Guid OrderId { get; set;}
        
        /// <summary>
		/// 商品Id
		/// </summary>
        public Guid GoodsId { get; set;}
        
        /// <summary>
		/// 单品Id
		/// </summary>
        public Guid SingleGoodsId { get; set;}
        
        /// <summary>
		/// 数量
		/// </summary>
        public int Quantity { get; set;}

        /// <summary>
        /// 商品名称
        /// </summary>
        [MaxLength(200)]
        public string GoodsName { get; set;}

        /// <summary>
        /// 商品编号
        /// </summary>
        [MaxLength(50)]
        public string GoodsNo { get; set;}

        /// <summary>
        /// 属性组合值
        /// </summary>
        [MaxLength(500)]
        public string GoodsAttribute { get; set;}

        /// <summary>
        /// 单品编号
        /// </summary>
        [MaxLength(50)]
        public string SingleGoodsNo { get; set;}

        /// <summary>
        /// 单位
        /// </summary>
        [MaxLength(10)]
        public string Unit { get; set;}
        
        /// <summary>
		/// 
		/// </summary>
        public decimal Price { get; set;}
        
        /// <summary>
		/// 是否实物
		/// </summary>
        public bool IsReal { get; set; }

        /// <summary>
        /// 是否免邮
        /// </summary>
        public bool FreeShipping { get; set; }

        [NotMapped]
        public StorageFile GoodsImage { get; set; }

        /// <summary>
        /// 退款状态
        /// </summary>
        public OrderRefundStatus RefundStatus { get; set; }

        /// <summary>
        /// 单品重量
        /// </summary>
        public decimal Weight { get; set; }

        /// <summary>
        /// 秒杀关联Id
        /// </summary>
        public Guid? LimitGoodsId { get; set; } = Guid.Empty;
    }
}
