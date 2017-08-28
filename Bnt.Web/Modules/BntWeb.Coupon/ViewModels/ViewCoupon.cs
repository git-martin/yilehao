using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BntWeb.Coupon.Models;

namespace BntWeb.Coupon.ViewModels
{
    public class ViewCoupon
    {
        public Guid Id { get; set; }

        /// <summary>
        /// 优惠券类型
        /// </summary>
        public CouponType CouponType { set; get; }
        /// <summary>
        /// 优惠券名称
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 优惠金额
        /// </summary>
        public decimal Money { get; set; }

        /// <summary>
        /// 最低消费金额
        /// </summary>
        public decimal Minimum { get; set; }
        /// <summary>
        /// 折扣，折扣券
        /// </summary>
        public decimal Discount { set; get; }

        /// <summary>
        /// 有效期类型，1指定天，2指定日期
        /// </summary>
        public ExpiryType ExpiryType { set; get; }

        /// <summary>
        /// 有效期限天数，0无限期，与StartTime不可重复
        /// </summary>
        public int? ExpiryDay { get; set; }

        /// <summary>
        /// 有效期开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 有效期截止时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 使用说明
        /// </summary>
        public string Describe { get; set; }

        /// <summary>
        /// 可领取优惠券数量
        /// </summary>
        public int Quantity { get; set; }
    }
    public class SendCoupon
    {
        public int MemberType { set; get; }
        public Guid MemberId { set; get; }
        public string UserName { set; get; }

        public Guid CouponId { set; get; }

        public int SentType { set; get; }
    }
}