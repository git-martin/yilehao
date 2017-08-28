/* 
    ======================================================================== 
        File name：        OrderModel
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/7/7 11:22:05
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using BntWeb.Data.Services;
using BntWeb.Environment;
using BntWeb.Feedback.Services;
using BntWeb.FileSystems.Media;
using BntWeb.Mall.Models;
using BntWeb.OrderProcess;
using BntWeb.OrderProcess.Models;
using BntWeb.OrderProcess.Services;
using BntWeb.Utility.Extensions;

namespace BntWeb.Mall.ApiModels
{
    public class OrderModel
    {
        /// <summary>
        /// 收货人
        /// </summary>
        public string Consignee { get; set; }

        /// <summary>
        /// 省份编号
        /// </summary>
        public string Province { get; set; }

        /// <summary>
        /// 城市编号
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// 区县编号
        /// </summary>
        public string District { get; set; }

        /// <summary>
        /// 街道/乡镇编号
        /// </summary>
        public string Street { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 四级行政区全称的合集
        /// </summary>
        public string RegionName { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string Tel { get; set; }

        /// <summary>
        /// 最佳送货时间
        /// </summary>
        public string BestTime { get; set; }

        /// <summary>
        /// 订单备注
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 折抵积分数量
        /// </summary>
        public int Integral { get; set; }

        ///// <summary>
        ///// 余额付款
        ///// </summary>
        //public bool UseBalance { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<OrderGoodsModel> Goods { get; set; }


        /// <summary>
        /// 优惠券码
        /// </summary>
        public string CouponCode { set; get; }
    }

    public class OrderGoodsModel
    {
        public Guid Id { get; set; }

        public int Quantity { get; set; }
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
        /// 优惠券优惠金额
        /// </summary>
        public decimal CouponMoney { set; get; }
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

        ///// <summary>
        ///// 是否已经投诉
        ///// </summary>
        //public bool HasComplaint { get; set; }

        public bool CanRemind { get; set; } = false;

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
            GoodsAmount = order.GoodsAmount;
            IntegralMoney = order.IntegralMoney;
            CouponMoney = order.CouponMoney;
            PayFee = order.PayFee;
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
            OrderGoods = order.OrderGoods.Select(g => new SimpleOrderGoods(g, order)).ToList();

            //if (OrderStatus == OrderStatus.Completed)
            //{
            //    var feedbackService = HostConstObject.Container.Resolve<IFeedbackService>();
            //    int totalCount;
            //    HasComplaint =
            //        feedbackService.GetFeedbackListBySourceId(order.MemberId.ToGuid(), order.Id,
            //            Feedback.Models.FeedbackType.Complaint, "Order", 1, 1, out totalCount).Count > 0;
            //}

            if (order.OrderStatus == OrderStatus.WaitingForDelivery)
            {
                var orderService = HostConstObject.Container.Resolve<IOrderService>();
                var canRemind = orderService.CheckTodayCanRemind(order.Id,order.MemberId);
                CanRemind = canRemind;
            }
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

    public class OrderCalculationModel
    {
        /// <summary>
        /// 默认来源于购物车
        /// </summary>
        public bool IsFromCart { get; set; } = true;

        public Guid? AddressId { get; set; }

        public List<Guid> CartIds { get; set; }

        public List<SubmitOrderGoods> SingleGoods { get; set; }
    }

    public class SubmitOrderGoods
    {
        public Guid SingleGoodsId { get; set; }

        public int Quantity { get; set; }
    }

    /// <summary>
    /// 订单确认商品信息
    /// </summary>
    public class OrderCalculationGoodsModel
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
        /// 属性组合值
        /// </summary>
        public string GoodsAttribute { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 主图
        /// </summary>
        public SimplifiedStorageFile GoodsImage { set; get; }

        public OrderCalculationGoodsModel(Cart model)
        {
            GoodsId = model.GoodsId;
            SingleGoodsId = model.SingleGoodsId;
            Quantity = model.Quantity;
            GoodsName = model.GoodsName;
            GoodsAttribute = model.GoodsAttribute;
            Price = model.Price;

            var fileService = HostConstObject.Container.Resolve<IStorageFileService>();

            var goodsImage = fileService.GetFiles(SingleGoodsId, MallModule.Instance.InnerKey, GoodsId.ToString()).FirstOrDefault() ??
                             fileService.GetFiles(GoodsId, MallModule.Key, "MainImage").FirstOrDefault();
            GoodsImage = goodsImage?.Simplified();
        }

        public OrderCalculationGoodsModel(SingleGoods model, int quantity)
        {
            GoodsId = model.GoodsId;
            SingleGoodsId = model.Id;
            Quantity = quantity;
            GoodsName = model.Goods.Name;
            GoodsAttribute = string.Join(",", model.Attributes.Select(me => me.AttributeValue).ToList());
            Price = model.Goods.IsGroupon ? ((model.Goods.GrouponStartTime <= DateTime.Now && DateTime.Now <= model.Goods.GrouponEndTime) ? model.GrouponPrice : model.Price) : model.Price;

            var fileService = HostConstObject.Container.Resolve<IStorageFileService>();

            var goodsImage = fileService.GetFiles(SingleGoodsId, MallModule.Instance.InnerKey, GoodsId.ToString()).FirstOrDefault() ??
                             fileService.GetFiles(GoodsId, MallModule.Key, "MainImage").FirstOrDefault();
            GoodsImage = goodsImage?.Simplified();
        }

    }
}