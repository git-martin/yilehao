using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using BntWeb.Environment;
using BntWeb.FileSystems.Media;
using BntWeb.OrderProcess.Models;

namespace BntWeb.OrderProcess.ViewModels
{

    public class OrderModelView
    {
    }


    public class SimpleOderModel
    {
        public Guid Id { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public OrderStatus OrderStatus { get; set; }

        /// <summary>
        /// 物流状态
        /// </summary>
        public ShippingStatus ShippingStatus { get; set; }

        /// <summary>
        /// 订单总额=商品总价+物流费用
        /// </summary>
        public decimal OrderAmount { get; set; }

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
        /// 物流费用
        /// </summary>
        public decimal ShippingFee { get; set; }
        /// <summary>
        /// 商品总价
        /// </summary>
        public decimal GoodsAmount { get; set; }
        /// <summary>
        /// 积分折抵金额
        /// </summary>
        public decimal IntegralMoney { get; set; }
        /// <summary>
        /// 实付总额=订单总额-折扣-折抵
        /// </summary>
        public decimal PayFee { get; set; }
        /// <summary>
        /// 收货人
        /// </summary>
        public string Consignee { get; set; }

        /// <summary>
        /// 行政区划名
        /// </summary>
        public string RegionName { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string Tel { get; set; }

        ///// <summary>
        ///// 最佳送货时间
        ///// </summary>
        //public string BestTime { get; set; }

        /// <summary>
        /// 物流方式名称
        /// </summary>
        public string ShippingName { get; set; }

        /// <summary>
        /// 物流公司代码
        /// </summary>
        public string ShippingCode { get; set; }

        /// <summary>
        /// 快递单号
        /// </summary>
        public string ShippingNo { get; set; }

        /// <summary>
        /// 发货时间
        /// </summary>
        public DateTime? ShippingTime { get; set; }

        /// <summary>
        /// 订单备注
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 下单时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 会员名称
        /// </summary>
        public string MemberName { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        public string PaymentName { get; set; }

        ///// <summary>
        ///// 是否已经投诉
        ///// </summary>
        //public bool HasComplaint { get; set; }

        public List<SimpleOrderGoods> OrderGoods { get; set; }

        public SimpleOderModel(Order order)
        {
            Id = order.Id;
            OrderNo = order.OrderNo;
            OrderStatus = order.OrderStatus;
            ShippingStatus = order.ShippingStatus;
            PayStatus = order.PayStatus;
            EvaluateStatus = order.EvaluateStatus;
            RefundStatus = order.RefundStatus;
            ShippingFee = order.ShippingFee;
            OrderAmount = order.OrderAmount;
            GoodsAmount = order.GoodsAmount;
            IntegralMoney = order.IntegralMoney;
            PayFee = order.PayFee;
            PaymentName = order.PaymentName;
            Consignee = order.Consignee;
            RegionName = order.PCDS;
            Address = order.Address;
            Tel = order.Tel;
            //BestTime = order.BestTime;
            Memo = order.Memo;
            ShippingName = order.ShippingName;
            ShippingCode = order.ShippingCode;
            ShippingNo = order.ShippingNo;
            ShippingTime = order.ShippingTime;
            CreateTime = order.CreateTime;
            MemberName = order.MemberName;

            OrderGoods = order.OrderGoods?.Select(g => new SimpleOrderGoods(g, order)).ToList();
        }
    }

    public class SimpleOrderGoods
    {

        /// <summary>
        /// 商品Id
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 单品Id
        /// </summary>
        public Guid SingleGoodsId { get; set; }

        /// <summary>
		/// 数量
		/// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 属性组合值
        /// </summary>
        public string GoodsAttribute { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 最大可退款金额
        /// </summary>
        public decimal MaxRefundAmount { get; set; }

        /// <summary>
        /// 退款状态
        /// </summary>
        public OrderRefundStatus RefundStatus { get; set; }

        public SimplifiedStorageFile GoodsImage { get; set; }

        public SimpleOrderGoods(OrderGoods goods, Order order)
        {
            GoodsId = goods.GoodsId;
            SingleGoodsId = goods.SingleGoodsId;
            Quantity = goods.Quantity;
            GoodsName = goods.GoodsName;
            GoodsAttribute = goods.GoodsAttribute;
            Unit = goods.Unit;
            Price = goods.Price;
            RefundStatus = goods.RefundStatus;
            var fileService = HostConstObject.Container.Resolve<IStorageFileService>();
            var goodsImage = fileService.GetFiles(goods.Id, OrderProcessModule.Key, "GoodsImage").FirstOrDefault();
            GoodsImage = goodsImage?.Simplified();

            MaxRefundAmount = Price * Quantity;
            if (order.IntegralMoney > 0)
            {
                MaxRefundAmount -= order.IntegralMoney * (Price * Quantity / order.GoodsAmount);
            }
        }
    }


    public class OrdreGoodsSimple
    {
        public Guid Id { get; set; }

        /// <summary>
        /// 商品Id
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 单品Id
        /// </summary>
        public Guid SingleGoodsId { get; set; }

        /// <summary>
		/// 数量
		/// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 属性组合值
        /// </summary>
        public string GoodsAttribute { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 最大可退款金额
        /// </summary>
        public decimal MaxRefundAmount { get; set; }

        /// <summary>
        /// 退款状态
        /// </summary>
        public OrderRefundStatus RefundStatus { get; set; }

        public SimplifiedStorageFile GoodsImage { get; set; }

        public OrdreGoodsSimple(OrderGoods goods)
        {
            Id = goods.Id;
            GoodsId = goods.GoodsId;
            SingleGoodsId = goods.SingleGoodsId;
            Quantity = goods.Quantity;
            GoodsName = goods.GoodsName;
            GoodsAttribute = goods.GoodsAttribute;
            Unit = goods.Unit;
            Price = goods.Price;
            RefundStatus = goods.RefundStatus;
            var fileService = HostConstObject.Container.Resolve<IStorageFileService>();
            var goodsImage = fileService.GetFiles(goods.Id, OrderProcessModule.Key, "GoodsImage").FirstOrDefault();
            GoodsImage = goodsImage?.Simplified();

            MaxRefundAmount = Price * Quantity;
        }
    }
}