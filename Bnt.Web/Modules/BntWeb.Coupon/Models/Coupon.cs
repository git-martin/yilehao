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
    [Table(KeyGenerator.TablePrefix + "Coupons")]
    public class Coupon
    {
        /// <summary>
        /// 优惠券Id
        /// </summary>
        [Key]
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
        /// 优惠金额，满减券
        /// </summary>
        public decimal Money { get; set; }

        /// <summary>
        /// 最低消费金额，满减券
        /// </summary>
        public decimal Minimum { get; set; }

        /// <summary>
        /// 折扣，折扣券
        /// </summary>
        public decimal Discount { set; get; } = 10;


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

        /// <summary>
        /// 状态：0禁用，1启用
        /// </summary>
        public CouponStatus Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

       

    }

    public enum CouponType
    {
        /// <summary>
        /// 满减券
        /// </summary>
        [Description("满减券")]
        FullCut=1,

        /// <summary>
        /// 打折券
        /// </summary>
        [Description("打折券")]
        Discount =2

    }

    public enum ExpiryType
    {
        /// <summary>
        /// 指定天
        /// </summary>
        [Description("指定天")]
        ExpiryByDay = 1,
        /// <summary>
        /// 指定日期
        /// </summary>
        [Description("指定日期")]
        ExpiryByDate = 2,
    }

    public enum CouponStatus
    {
        /// <summary>
        /// 启用
        /// </summary>
        [Description("启用")]
        Enable = 1,
        /// <summary>
        /// 禁用
        /// </summary>
        [Description("禁用")]
        Disable = 0
    }

    
}