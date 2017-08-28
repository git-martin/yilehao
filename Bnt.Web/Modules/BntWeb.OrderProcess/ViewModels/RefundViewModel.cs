using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Autofac;
using System.Web.UI.WebControls;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.Environment;
using BntWeb.OrderProcess.Models;

namespace BntWeb.OrderProcess.ViewModels
{
    public class RefundViewModel
    {
        public Guid Id { get; set; }

        /// <summary>
        /// 退款申请编号
        /// </summary>
        public string RefundNo { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 属性组合值
        /// </summary>
        public string GoodsAttribute { get; set; }

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

        public RefundViewModel(OrderRefund refund)
        {
            Id = refund.Id;
            RefundNo = refund.RefundNo;
            var currencyService = HostConstObject.Container.Resolve<ICurrencyService>();
            var goods= currencyService.GetSingleByConditon<OrderGoods>(a=>a.OrderId==refund.OrderId && a.SingleGoodsId==refund.SingleGoodsId);
            GoodsName = goods.GoodsName;
            GoodsAttribute = goods.GoodsAttribute;
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


   
}