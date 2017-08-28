/* 
    ======================================================================== 
        File name：        Payment
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/7/13 11:29:31
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
    [Table(KeyGenerator.TablePrefix + "Payments")]
    public class Payment
    {
        [Key]
        public Guid Id { get; set; }
        
        [MaxLength(50)]
        public string Code { get; set; }

        [MaxLength(120)]
        public string Name { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }
        
        public bool Enabled { get; set; }
    }

    public enum PaymentType
    {
        /// <summary>
        /// 余额支付
        /// </summary>
        [Description("余额支付")]
        Balance=0,
        /// <summary>
        /// 支付宝
        /// </summary>
        [Description("支付宝")]
        Alipay = 1,
        /// <summary>
        /// 微信
        /// </summary>
        [Description("微信")]
        WeiXin = 2
    }
}