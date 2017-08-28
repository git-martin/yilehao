/* 
    ======================================================================== 
        File name：        PaymentLog
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/7/13 10:19:26
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using BntWeb.Data;

namespace BntWeb.PaymentProcess.Models
{
    [Table(KeyGenerator.TablePrefix + "Pay_Logs")]
    public class PayLog
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(40)]
        public string TransactionNo { get; set; }

        public Guid OrderId { get; set; }

        [MaxLength(30)]
        public string OrderNo { get; set; }

        public decimal OrderAmount { get; set; }

        public Guid PaymentId { get; set; }

        [MaxLength(120)]
        public string PaymentName { get; set; }

        public LogStatus LogStatus { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime? PayTime { get; set; }

        [ForeignKey("PaymentId")]
        public virtual Payment Payment { get; set; }

    }

    public enum LogStatus
    {
        /// <summary>
        /// 未付款
        /// </summary>
        [Description("未付款")]
        Unpaid = 0,

        /// <summary>
        /// 已付款
        /// </summary>
        [Description("付款中")]
        Paying = 1,

        /// <summary>
        /// 已付款
        /// </summary>
        [Description("已付款")]
        Paid = 2,

        /// <summary>
        /// 已取消
        /// </summary>
        [Description("已取消")]
        Canceled = 3
    }
}