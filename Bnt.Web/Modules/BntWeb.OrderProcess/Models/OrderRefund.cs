using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using BntWeb.Data;

namespace BntWeb.OrderProcess.Models
{
    [Table(KeyGenerator.TablePrefix + "Order_Refunds")]
    public class OrderRefund
    {
        /// <summary>
        /// 
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 退款编号
        /// </summary>
        [MaxLength(30)]
        public string RefundNo { get; set; }

        /// <summary>
        /// 订单Id
        /// </summary>
        public Guid OrderId { get; set; }

        /// <summary>
        /// 单品id
        /// </summary>
        public Guid SingleGoodsId { get; set; }

        /// <summary>
        /// 退款类型
        /// </summary>
        public RefundType RefundType { get; set; }

        /// <summary>
        /// 退款状态
        /// </summary>
        public RefundStatus RefundStatus { get; set; }

        /// <summary>
        /// 退款金额
        /// </summary>
        public decimal RefundAmount { get; set; }

        /// <summary>
        /// 退款理由
        /// </summary>
        [MaxLength(500)]
        public string Reason { get; set; }

        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        [MaxLength(36)]
        public string ReviewUserId { get; set; }

        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? ReviewTime { get; set; }

        /// <summary>
        /// 审核结果
        /// </summary>
        public ReviewResult ReviewResult { get; set; }

        /// <summary>
        /// 审核说明
        /// </summary>
        [MaxLength(500)]
        public string ReviemMemo { get; set; }

        /// <summary>
        /// 物流名称
        /// </summary>
        [MaxLength(20)]
        public string ShippingName { get; set; }

        /// <summary>
        /// 物流编号
        /// </summary>
        [MaxLength(20)]
        public string ShippingNo { get; set; }

        /// <summary>
        /// 发货时间
        /// </summary>
        public DateTime? ShippingTime { get; set; }

        /// <summary>
        /// 物流说明
        /// </summary>
        [MaxLength(500)]
        public string ShippingMemo { get; set; }

        /// <summary>
        /// 打款人
        /// </summary>
        [MaxLength(36)]
        public string PayUserId { get; set; }

        /// <summary>
        /// 打款时间
        /// </summary>
        public DateTime? PayTime { get; set; }
    }

    [Table(KeyGenerator.TablePrefix + "View_Order_Refund")]
    public class ViewOrderRefund
    {
        /// <summary>
        /// 
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 退款编号
        /// </summary>
        [MaxLength(30)]
        public string RefundNo { get; set; }

        /// <summary>
        /// 订单Id
        /// </summary>
        public Guid OrderId { get; set; }

        /// <summary>
        /// 单品id
        /// </summary>
        public Guid SingleGoodsId { get; set; }

        /// <summary>
        /// 退款类型
        /// </summary>
        public RefundType RefundType { get; set; }

        /// <summary>
        /// 退款状态
        /// </summary>
        public RefundStatus RefundStatus { get; set; }

        /// <summary>
        /// 退款金额
        /// </summary>
        public decimal RefundAmount { get; set; }

        /// <summary>
        /// 退款理由
        /// </summary>
        [MaxLength(500)]
        public string Reason { get; set; }

        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        [MaxLength(36)]
        public string ReviewUserId { get; set; }

        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? ReviewTime { get; set; }

        /// <summary>
        /// 审核结果
        /// </summary>
        public ReviewResult ReviewResult { get; set; }

        /// <summary>
        /// 审核说明
        /// </summary>
        [MaxLength(500)]
        public string ReviemMemo { get; set; }

        /// <summary>
        /// 物流名称
        /// </summary>
        [MaxLength(20)]
        public string ShippingName { get; set; }

        /// <summary>
        /// 物流编号
        /// </summary>
        [MaxLength(20)]
        public string ShippingNo { get; set; }

        /// <summary>
        /// 发货时间
        /// </summary>
        public DateTime? ShippingTime { get; set; }

        /// <summary>
        /// 物流说明
        /// </summary>
        [MaxLength(500)]
        public string ShippingMemo { get; set; }

        /// <summary>
        /// 打款人
        /// </summary>
        [MaxLength(36)]
        public string PayUserId { get; set; }

        /// <summary>
        /// 打款时间
        /// </summary>
        public DateTime? PayTime { get; set; }

        public string OrderNo { set; get; }
        public decimal OrderAmount { set; get; }
        public decimal PayFee { set; get; }

        public string Consignee { set; get; }
        public string Tel { set; get; }
        public string MemberName { set; get; }
        public string Address { set; get; }
        public string Province { set; get; }
        public string City { set; get; }
        public string District { set; get; }
        public string Street { set; get; }
        public decimal ShippingFee { set; get; }
        public decimal IntegralMoney { set; get; }
        public decimal GoodsAmount { set; get; }
        public OrderStatus OrderStatus { set; get; }
        public OrderRefundStatus OrderRefundStatus { set; get; }
    }
    /// <summary>
    /// 退款账户类型：1支付宝，2银行卡
    /// </summary>
    public enum EnumRefundAccountType
    {
        /// <summary>
        /// 1支付宝
        /// </summary>
        [Description("1支付宝")]
        Alipay = 1,

        /// <summary>
        /// 2银行卡
        /// </summary>
        [Description("2银行卡")]
        Bank = 2
    }


    /// <summary>
    /// 退款类型
    /// </summary>
    public enum RefundType
    {
        /// <summary>
        /// 仅退款
        /// </summary>
        [Description("仅退款")]
        OnlyRefund = 0,

        /// <summary>
        /// 退款并退货
        /// </summary>
        [Description("退款并退货")]
        RefundAndReturn = 1
    }

    /// <summary>
    /// 审核结果
    /// </summary>
    public enum ReviewResult
    {
        /// <summary>
        /// 不通过
        /// </summary>
        [Description("不通过")]
        UnPassed = 0,

        /// <summary>
        /// 通过
        /// </summary>
        [Description("通过")]
        Passed = 1

        
    }

    public enum RefundStatus
    {
        /// <summary>
        /// 申请中
        /// </summary>
        [Description("申请中")]
        Applying = 0,
        /// <summary>
        /// 已撤销
        /// </summary>
        [Description("已撤销")]
        Revoked = 1,
        /// <summary>
        /// 已处理
        /// </summary>
        [Description("已处理")]
        Processed = 2,
        /// <summary>
        /// 已完成
        /// </summary>
        [Description("已完成")]
        Completed = 3

    }
}