using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BntWeb.FileSystems.Media;
using BntWeb.OrderProcess.Models;

namespace BntWeb.OrderProcess.ApiModels
{
    public class RefundApplyModel
    {
        /// <summary>
        /// 订单id
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
        /// 退款金额
        /// </summary>
        public decimal RefundAmount { get; set; }
        /// <summary>
        /// 退款原因
        /// </summary>
        [MaxLength(500)]
        public string Reason { get;set; }
    }

    public class RefundApplyUpdateModel
    {
        /// <summary>
        /// 退款申请id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 退款类型
        /// </summary>
        public RefundType RefundType { get; set; }
        /// <summary>
        /// 退款金额
        /// </summary>
        public decimal RefundAmount { get; set; }
        /// <summary>
        /// 退款原因
        /// </summary>
        [MaxLength(500)]
        public string Reason { get; set; }
    }

    /// <summary>
    /// 退款申请物流信息
    /// </summary>
    public class RefundLogisticsModel
    {
        /// <summary>
        /// 退款申请id
        /// </summary>
        public Guid Id { get; set; }

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
        /// 物流说明
        /// </summary>
        [MaxLength(500)]
        public string ShippingMemo { get; set; }
    }

    /// <summary>
    /// 退款申请详情
    /// </summary>
    public class RefundApplyInfoModel
    {
        /// <summary>
        /// 退款申请id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 退款编号
        /// </summary>
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
        public string Reason { get; set; }

        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime CreateTime { get; set; }

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
        public string ReviemMemo { get; set; }

        /// <summary>
        /// 物流名称
        /// </summary>
        public string ShippingName { get; set; }

        /// <summary>
        /// 物流编号
        /// </summary>
        public string ShippingNo { get; set; }

        /// <summary>
        /// 发货时间
        /// </summary>
        public DateTime? ShippingTime { get; set; }

        /// <summary>
        /// 物流说明
        /// </summary>
        public string ShippingMemo { get; set; }
        public RefundApplyInfoModel(OrderRefund refund)
        {
            Id = refund.Id;
            RefundNo = refund.RefundNo;
            OrderId = refund.OrderId;
            SingleGoodsId = refund.SingleGoodsId;
            RefundType = refund.RefundType;
            RefundStatus = refund.RefundStatus;
            RefundAmount = refund.RefundAmount;
            Reason = refund.Reason;
            CreateTime = refund.CreateTime;
            ReviewTime = refund.ReviewTime;
            ReviewResult = refund.ReviewResult;
            ReviemMemo = refund.ReviemMemo;
            ShippingName = refund.ShippingName;
            ShippingNo = refund.ShippingNo;
            ShippingTime = refund.ShippingTime;
            ShippingMemo = refund.ShippingMemo;
        }
    }

    public class OrderSingleGoodsModel
    {
        /// <summary>
        /// 订单商品id
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 订单单品id
        /// </summary>
        public Guid SingleGoodsId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 属性组合值
        /// </summary>
        public string GoodsAttribute { get; set; }

        /// <summary>
        /// 商品价格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 主图
        /// </summary>
        public SimplifiedStorageFile MainImage { set; get; }
    }
}