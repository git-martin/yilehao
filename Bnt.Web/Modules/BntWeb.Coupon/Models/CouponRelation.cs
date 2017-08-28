using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using BntWeb.Data;
using System.ComponentModel;
using BntWeb.FileSystems.Media;

namespace BntWeb.Coupon.Models
{
    [Table(KeyGenerator.TablePrefix + "Coupon_Relations")]
    public class CouponRelation
    {
        /// <summary>
        /// Id
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 优惠券Id
        /// </summary>
        public Guid CouponId { get; set; }

        /// <summary>
        /// 优惠码
        /// </summary>
        public string CouponCode { get; set; }

        /// <summary>
        /// 领取人Id
        /// </summary>
        public string MemberId { get; set; }
        
        /// <summary>
        /// 有效期开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 有效期截止时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 领取时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 是否使用
        /// </summary>
        public bool IsUsed { get; set; }

       /// <summary>
       /// 使用时间
       /// </summary>
        public DateTime UsedTime { get; set; }

        /// <summary>
        /// 使用源Id
        /// </summary>
        public Guid SourceId { get; set; }

        /// <summary>
        /// 使用类型
        /// </summary>
        public string SourceType { get; set; }

        public EnumFromType FromType { set; get; }
        [ForeignKey("CouponId")]
        public virtual Coupon Coupon { get; set; }

        public enum EnumFromType
        {
            /// <summary>
            /// 领取
            /// </summary>
            [Description("领取")]
            Get = 1,
            /// <summary>
            /// 发放
            /// </summary>
            [Description("发放")]
            Send = 2,
            /// <summary>
            /// 赠送
            /// </summary>
            [Description("赠送")]
            Gift = 3,
        }

    }
}