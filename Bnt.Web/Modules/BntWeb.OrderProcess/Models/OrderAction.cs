
/* Models Code Auto-Generation 
    ======================================================================== 
        File name：		OrderActions
        Module:			
        Author：		罗嗣宝
        Create Time：		2016/7/6 16:49:29
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

namespace BntWeb.OrderProcess.Models
{
    /// <summary>
    /// 实体：OrderActions
    /// </summary>
    [Table(KeyGenerator.TablePrefix + "Order_Actions")]
    public class OrderAction
    {
        /// <summary>
        /// 
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 订单Id
        /// </summary>
        public Guid OrderId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [MaxLength(36)]
        public string UserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [MaxLength(256)]
        public string UserName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public OrderStatus OrderStatus { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ShippingStatus ShippingStatus { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public PayStatus PayStatus { get; set; }

        /// <summary>
        /// 退款状态
        /// </summary>
        public OrderRefundStatus RefundStatus { get; set; }

        /// <summary>
        /// 评价状态
        /// </summary>
        public EvaluateStatus EvaluateStatus { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [MaxLength(100)]
        public string Memo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateTime { get; set; }

    }
}
