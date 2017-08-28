
/* Models Code Auto-Generation 
    ======================================================================== 
        File name：		Orders
        Module:			
        Author：		罗嗣宝
        Create Time：		2016/7/6 16:49:29
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
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
    /// 实体：Orders
    /// </summary>
    [Table(KeyGenerator.TablePrefix + "Orders")]
    public class Order
    {
        /// <summary>
        /// 
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        [MaxLength(30)]
        public string OrderNo { get; set; }

        /// <summary>
        /// 下单人Id
        /// </summary>
        [MaxLength(36)]
        public string MemberId { get; set; }

        /// <summary>
        /// 下单人用户名
        /// </summary>
        [MaxLength(256)]
        public string MemberName { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public OrderStatus OrderStatus { get; set; }

        /// <summary>
        /// 物流状态
        /// </summary>
        public ShippingStatus ShippingStatus { get; set; }

        /// <summary>
        /// 支付状态
        /// </summary>
        public PayStatus PayStatus { get; set; }

        /// <summary>
        /// 评价状态
        /// </summary>
        public EvaluateStatus EvaluateStatus { get; set; }
        /// <summary>
        /// 退款状态
        /// </summary>
        public OrderRefundStatus RefundStatus { get; set; }

        /// <summary>
        /// 收货人
        /// </summary>
        [MaxLength(60)]
        public string Consignee { get; set; }

        /// <summary>
        /// 省份编号
        /// </summary>
        [MaxLength(10)]
        public string Province { get; set; }

        /// <summary>
        /// 城市编号
        /// </summary>
        [MaxLength(10)]
        public string City { get; set; }

        /// <summary>
        /// 区县编号
        /// </summary>
        [MaxLength(10)]
        public string District { get; set; }

        /// <summary>
        /// 街道/乡镇编号
        /// </summary>
        [MaxLength(10)]
        public string Street { get; set; }

        /// <summary>
        /// 四级行政区全称的合集
        /// </summary>
        [MaxLength(200)]
        public string PCDS { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        [MaxLength(200)]
        public string Address { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        [MaxLength(100)]
        public string Tel { get; set; }

        /// <summary>
        /// 最佳送货时间
        /// </summary>
        [MaxLength(100)]
        public string BestTime { get; set; }

        /// <summary>
        /// 是否需要物流
        /// </summary>
        public bool NeedShipping { get; set; }

        /// <summary>
        /// 物流方式Id
        /// </summary>
        public Guid ShippingId { get; set; }

        /// <summary>
        /// 物流方式名称
        /// </summary>
        [MaxLength(120)]
        public string ShippingName { get; set; }

        /// <summary>
        /// 物流公司代码
        /// </summary>
        [MaxLength(120)]
        public string ShippingCode { get; set; }

        /// <summary>
        /// 快递单号
        /// </summary>
        [MaxLength(30)]
        public string ShippingNo { get; set; }

        /// <summary>
        /// 是否在线支付
        /// </summary>
        public bool PayOnline { get; set; }

        /// <summary>
        /// 支付方式Id
        /// </summary>
        public Guid PaymentId { get; set; }

        /// <summary>
        /// 支付方式名称
        /// </summary>
        [MaxLength(120)]
        public string PaymentName { get; set; }

        /// <summary>
        /// 商品总价
        /// </summary>
        public decimal GoodsAmount { get; set; }

        /// <summary>
        /// 物流费用
        /// </summary>
        public decimal ShippingFee { get; set; }

        /// <summary>
        /// 订单总额=商品总价+物流费用
        /// </summary>
        public decimal OrderAmount { get; set; }

        /// <summary>
        /// 使用积分数量
        /// </summary>
        public int Integral { get; set; }

        /// <summary>
        /// 积分折抵金额
        /// </summary>
        public decimal IntegralMoney { get; set; }


        /// <summary>
        /// 优惠券优惠金额
        /// </summary>
        public decimal CouponMoney { set; get; }

        /// <summary>
        /// 实付总额=订单总额-折扣-折抵
        /// </summary>
        public decimal PayFee { get; set; }

        /// <summary>
        /// 余额付款
        /// </summary>
        public decimal BalancePay { get; set; }

        /// <summary>
        /// 未支付总额=实付总额-余额付款
        /// </summary>
        public decimal UnpayFee { get; set; }

        /// <summary>
        /// 下单时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTime? PayTime { get; set; }

        /// <summary>
        /// 发货时间
        /// </summary>
        public DateTime? ShippingTime { get; set; }

        /// <summary>
        /// 订单备注
        /// </summary>
        [MaxLength(200)]
        public string Memo { get; set; }

        /// <summary>
        /// 退款金额
        /// </summary>
        public decimal RefundFee { get; set; }

        [MaxLength(50)]
        public string ModuleKey { get; set; }


        [MaxLength(50)]
        public string ModuleName { get; set; }


        [MaxLength(20)]
        public string SourceType { get; set; }


        public virtual List<OrderGoods> OrderGoods { get; set; }
        public virtual List<OrderAction> OrderActions { get; set; }

    }

    public enum OrderStatus
    {
        /// <summary>
        /// 已删除
        /// </summary>
        [Description("已删除")]
        Deleted = -1,

        /// <summary>
        /// 待付款
        /// </summary>
        [Description("待付款")]
        PendingPayment = 0,

        /// <summary>
        /// 待发货
        /// </summary>
        [Description("待发货")]
        WaitingForDelivery = 1,

        /// <summary>
        /// 待收货
        /// </summary>
        [Description("待收货")]
        WaitingForReceiving = 2,

        /// <summary>
        /// 已完成
        /// </summary>
        [Description("已完成")]
        Completed = 3,

        /// <summary>
        /// 已关闭
        /// </summary>
        [Description("已关闭")]
        Closed = 4

    }

    public enum ShippingStatus
    {
        /// <summary>
        /// 未发货
        /// </summary>
        [Description("未发货")]
        Unshipped = 0,

        /// <summary>
        /// 已发货
        /// </summary>
        [Description("已发货")]
        Shipped = 1
    }

    public enum PayStatus
    {
        /// <summary>
        /// 未付款
        /// </summary>
        [Description("未付款")]
        Unpaid = 0,

        /// <summary>
        /// 付款中
        /// </summary>
        [Description("付款中")]
        Paying = 1,

        /// <summary>
        /// 已付款
        /// </summary>
        [Description("已付款")]
        Paid = 2
    }

    public enum EvaluateStatus
    {
        /// <summary>
        /// 未评价
        /// </summary>
        [Description("未评价")]
        NotEvaluated = 0,

        /// <summary>
        /// 已评价
        /// </summary>
        [Description("已评价")]
        Evaluated = 1,

        /// <summary>
        /// 已回复
        /// </summary>
        [Description("已回复")]
        Replied = 2
    }

    /// <summary>
    /// 退款状态
    /// </summary>
    public enum OrderRefundStatus
    {
        /// <summary>
        /// 未退款
        /// </summary>
        [Description("未退款")]
        NoRefund = 0,

        /// <summary>
        /// 退款中
        /// </summary>
        [Description("退款中")]
        Refunding = 1,

        /// <summary>
        /// 已退款
        /// </summary>
        [Description("已退款")]
        Refunded = 2
    }
}