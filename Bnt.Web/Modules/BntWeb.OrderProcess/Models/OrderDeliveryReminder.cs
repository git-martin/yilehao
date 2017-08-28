using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using BntWeb.Data;

namespace BntWeb.OrderProcess.Models
{
    /// <summary>
    /// 实体：OrderDeliveryReminders
    /// </summary>
    [Table(KeyGenerator.TablePrefix + "Order_Delivery_Reminders")]
    public class OrderDeliveryReminder
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
        /// 订单编号
        /// </summary>
        [MaxLength(30)]
        public string OrderNo { get; set; }

        /// <summary>
        /// 提醒人Id
        /// </summary>
        [MaxLength(36)]
        public string MemberId { get; set; }

        /// <summary>
        /// 提醒人用户名
        /// </summary>
        [MaxLength(256)]
        public string MemberName { get; set; }

        /// <summary>
        /// 提醒时间
        /// </summary>
        public DateTime CreateTime { get; set; }

       
    }

}