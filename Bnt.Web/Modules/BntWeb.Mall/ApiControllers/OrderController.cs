using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Http;
using BntWeb.Config.Models;
using BntWeb.Coupon.Models;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.FileSystems.Media;
using BntWeb.Logistics.Services;
using BntWeb.Mall.ApiModels;
using BntWeb.Mall.Models;
using BntWeb.Mall.Services;
using BntWeb.MemberBase.Models;
using BntWeb.OrderProcess.Models;
using BntWeb.OrderProcess.Services;
using BntWeb.Services;
using BntWeb.Utility.Extensions;
using BntWeb.Validation;
using BntWeb.Wallet.Services;
using BntWeb.WebApi.Filters;
using BntWeb.WebApi.Models;

namespace BntWeb.Mall.ApiControllers
{
    public class OrderController : BaseApiController
    {
        private readonly IOrderService _orderService;
        private readonly IGoodsService _goodsService;
        private readonly ICurrencyService _currencyService;
        private readonly IStorageFileService _storageFileService;
        private readonly IShippingAreaService _shippingAreaService;
        private readonly IWalletService _walletService;
        private readonly IConfigService _configService;

        public OrderController(IOrderService orderService, IGoodsService goodsService, ICurrencyService currencyService, IStorageFileService storageFileService, IShippingAreaService shippingAreaService, IWalletService walletService, IConfigService configService)
        {
            _orderService = orderService;
            _goodsService = goodsService;
            _currencyService = currencyService;
            _storageFileService = storageFileService;
            _shippingAreaService = shippingAreaService;
            _walletService = walletService;
            _configService = configService;
        }

        [HttpPost]
        [BasicAuthentication]
        public ApiResult Submit([FromBody]OrderModel submitOrder)
        {
            Argument.ThrowIfNullOrEmpty(submitOrder.Province, "收货地址省份");
            Argument.ThrowIfNullOrEmpty(submitOrder.City, "收货地址城市");
            Argument.ThrowIfNullOrEmpty(submitOrder.Address, "收货详细地址");
            Argument.ThrowIfNullOrEmpty(submitOrder.Consignee, "收货人");
            Argument.ThrowIfNullOrEmpty(submitOrder.Tel, "收货人电话");

            var notShipping = _shippingAreaService.NotShippingArea(submitOrder.City) ??
                             _shippingAreaService.NotShippingArea(submitOrder.Province);
            if (notShipping != null)
            {
                throw new WebApiInnerException("30001", "您提交的地址不在我们的配送范围内！");
            }

            if (submitOrder.Goods == null || submitOrder.Goods.Count < 1)
                throw new WebApiInnerException("0001", "订单中没有商品");

            var order = new Order
            {
                MemberId = AuthorizedUser.Id,
                MemberName = AuthorizedUser.UserName,
                OrderStatus = OrderStatus.PendingPayment,
                ShippingStatus = ShippingStatus.Unshipped,
                PayStatus = PayStatus.Unpaid,
                EvaluateStatus = EvaluateStatus.NotEvaluated,
                Consignee = submitOrder.Consignee,
                Province = submitOrder.Province,
                City = submitOrder.City,
                District = submitOrder.District,
                Street = submitOrder.Street,
                PCDS = submitOrder.RegionName,
                Address = submitOrder.Address,
                Tel = submitOrder.Tel,
                BestTime = submitOrder.BestTime,
                Memo = submitOrder.Memo,
                Integral = submitOrder.Integral,
                NeedShipping = true,
                PayOnline = true,
                CreateTime = DateTime.Now,
                ModuleKey = MallModule.Key,
                ModuleName = MallModule.DisplayName,
                SourceType = "Mall"
            };

            //加载商品
            var orderGoods = new List<OrderGoods>();

            foreach (var item in submitOrder.Goods)
            {
                var singleGoods = _goodsService.LoadFullSingleGoods(item.Id);
                if (singleGoods?.Goods == null || singleGoods.Goods.Status != GoodsStatus.InSale)
                    throw new WebApiInnerException("0002", "存在非法或者失效的商品");
                if (singleGoods.Stock < item.Quantity)
                    throw new WebApiInnerException("0003", $"【{singleGoods.Goods.Name}】库存不足");
                if (item.Quantity < 1)
                    throw new WebApiInnerException("0004", $"【{singleGoods.Goods.Name}】商品数量必须大于0");

                var goodsImage = _storageFileService.GetFiles(singleGoods.Id, MallModule.Instance.InnerKey, singleGoods.Goods.Id.ToString()).FirstOrDefault() ??
                                 _storageFileService.GetFiles(singleGoods.GoodsId, MallModule.Key, "MainImage").FirstOrDefault();
                orderGoods.Add(new OrderGoods
                {
                    GoodsId = singleGoods.Goods.Id,
                    GoodsNo = singleGoods.Goods.GoodsNo,
                    GoodsName = singleGoods.Goods.Name,
                    SingleGoodsId = singleGoods.Id,
                    SingleGoodsNo = singleGoods.SingleGoodsNo,
                    GoodsAttribute = string.Join(",", singleGoods.Attributes.Select(a => a.AttributeValue).ToArray()),
                    Unit = singleGoods.Unit,
                    // Price = singleGoods.Goods.IsGroupon ? (singleGoods.Goods.GrouponEndTime <= DateTime.Now ? singleGoods.Price : singleGoods.GrouponPrice) : singleGoods.Price,
                    Price = singleGoods.Goods.IsGroupon ? ((singleGoods.Goods.GrouponStartTime <= DateTime.Now && singleGoods.Goods.GrouponEndTime >= DateTime.Now) ? singleGoods.GrouponPrice : singleGoods.Price) : singleGoods.Price,
                    Quantity = item.Quantity,
                    IsReal = true,
                    GoodsImage = goodsImage,
                    FreeShipping = singleGoods.Goods.FreeShipping
                });
            }

            //计算商品总价
            order.GoodsAmount = orderGoods.Sum(g => g.Price * g.Quantity);
            //计算物流费用
            var goodsWeight = orderGoods.Sum(g => g.Weight);
            order.ShippingFee = orderGoods.Any(g => g.FreeShipping) ? 0 : _shippingAreaService.GetAreaFreight(submitOrder.Province, submitOrder.City, goodsWeight);
            //计算订单总价
            var result = new ApiResult();
            order.OrderAmount = order.GoodsAmount + order.ShippingFee;
            //事务控制
            using (TransactionScope scope = new TransactionScope())
            {
                //计算积分折抵
                if (order.Integral > 0)
                {
                    var integralWallet = _walletService.GetWalletByMemberId(order.MemberId, Wallet.Models.WalletType.Integral);
                    if (integralWallet == null || integralWallet.Available < order.Integral)
                        throw new WebApiInnerException("0003", "可用积分不足");
                    else
                    {
                        var systemConfig = _configService.Get<SystemConfig>();
                        //计算剩余积分总共可以抵扣多少钱
                        var maxDiscountMoney = (decimal)integralWallet.Available / 100 * systemConfig.DiscountRate;
                        if (maxDiscountMoney >= order.GoodsAmount)
                        {
                            order.Integral = (int)(order.GoodsAmount / systemConfig.DiscountRate * 100);
                        }

                        string error;
                        _walletService.Draw(order.MemberId, Wallet.Models.WalletType.Integral, order.Integral, "抵扣订单", out error);
                        if (string.IsNullOrWhiteSpace(error))
                        {
                            order.IntegralMoney = (decimal)order.Integral / 100 * systemConfig.DiscountRate;
                        }
                    }
                }
                //计算需要支付的费用
                order.PayFee = order.GoodsAmount - order.IntegralMoney + order.ShippingFee;

                #region 优惠券使用
                //优惠券使用
                if (!string.IsNullOrWhiteSpace(submitOrder.CouponCode))
                {
                    var couponRelation =
                        _currencyService.GetSingleByConditon<CouponRelation>(
                            me => me.CouponCode == submitOrder.CouponCode && me.MemberId == AuthorizedUser.Id);
                    if (couponRelation == null)
                        throw new WebApiInnerException("0041", "优惠券码无效");
                    if (couponRelation.IsUsed)
                        throw new WebApiInnerException("0042", "优惠券已使用");
                    if (couponRelation.StartTime > DateTime.Now)
                        throw new WebApiInnerException("0043", "优惠券没有到可用时间");
                    if (couponRelation.EndTime < DateTime.Now)
                        throw new WebApiInnerException("0044", "优惠券已经过期");


                    var coupon = _currencyService.GetSingleById<Coupon.Models.Coupon>(couponRelation.CouponId);
                    if (coupon == null || coupon.Status == 0)
                        throw new WebApiInnerException("0045", "优惠券不存在或已禁用");

                    bool isUsed = false;
                    //使用优惠券
                    if (coupon.CouponType == CouponType.FullCut)
                    {
                        if (order.GoodsAmount > coupon.Minimum)
                        {
                            order.OrderAmount = order.GoodsAmount - coupon.Money + order.ShippingFee;
                            order.CouponMoney = coupon.Money; //优惠金额
                            isUsed = true;
                        }
                    }
                    else
                    {
                        decimal afterCoupon = (order.GoodsAmount) * (coupon.Discount / Convert.ToDecimal(10)) + order.ShippingFee;
                        order.CouponMoney = order.OrderAmount - afterCoupon;//优惠金额

                        order.OrderAmount = afterCoupon;
                        isUsed = true;
                    }

                    if (isUsed)
                    {
                        couponRelation.IsUsed = true;
                        couponRelation.UsedTime = DateTime.Now;
                        couponRelation.SourceId = order.Id;
                        couponRelation.SourceType = "Order";
                        if (!_currencyService.Update(couponRelation))
                            throw new WebApiInnerException("0046", "优惠券使用失败");

                    }
                }
                #endregion

                if (order.PayFee == 0)
                {
                    order.OrderStatus = OrderStatus.WaitingForDelivery;
                    order.PayStatus = PayStatus.Paid;
                    order.PayTime = DateTime.Now;
                }
                else
                {
                    order.UnpayFee = order.PayFee;

                    ////使用余额付款
                    #region
                    //if (submitOrder.UseBalance)
                    //{
                    //    var cashWallet = _walletService.GetWalletByMemberId(order.MemberId.ToGuid(), Wallet.Models.WalletType.Cash);
                    //    if (cashWallet != null && cashWallet.Available > 0)
                    //    {
                    //        if (cashWallet.Available > order.PayFee)
                    //        {
                    //            string error;
                    //            _walletService.Draw(order.MemberId.ToGuid(), Wallet.Models.WalletType.Cash, order.PayFee,
                    //                "支付订单", out error);
                    //            if (string.IsNullOrWhiteSpace(error))
                    //            {
                    //                order.BalancePay = order.PayFee;
                    //                order.OrderStatus = OrderStatus.Confirmed;
                    //                order.PayStatus = PayStatus.Paid;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            string error;
                    //            _walletService.Draw(order.MemberId.ToGuid(), Wallet.Models.WalletType.Cash, cashWallet.Available,
                    //                "支付订单", out error);
                    //            if (string.IsNullOrWhiteSpace(error))
                    //            {
                    //                order.UnpayFee = order.PayFee - cashWallet.Available;
                    //                order.BalancePay = cashWallet.Available;
                    //            }
                    //        }
                    //    }
                    //}
                    #endregion
                }
                _orderService.SubmitOrder(order, orderGoods);

                //清理购物车相关数据
                foreach (var item in submitOrder.Goods)
                {
                    _currencyService.DeleteByConditon<Cart>(
                        c => c.MemberId.Equals(AuthorizedUser.Id) && c.SingleGoodsId.Equals(item.Id));
                }

                //提交
                scope.Complete();
            }
            //result.SetData(order);
            var data = new
            {
                order.Id,
                order.OrderNo,
                order.PayFee,
                order.CreateTime
            };
            result.SetData(data);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderType">0:所有，1：待付款，2：待发货，3：待评价</param>
        /// <param name="pageNo"></param>
        /// <param name="limit"></param>
        /// <param name="keywords"></param>
        /// <returns></returns>
        [HttpGet]
        [BasicAuthentication]
        public ApiResult MyOrders(int orderType = 0, int pageNo = 1, int limit = 10, string keywords = "")
        {
            OrderStatus? orderStatus = null;
            OrderStatus? extentOrderStatus = null;
            EvaluateStatus? evaluateStatus = null;
            OrderRefundStatus? refundStatus = null;
            OrderRefundStatus? extentRefundStatus = null;

            switch (orderType)
            {
                case 1:
                    {
                        orderStatus = OrderStatus.PendingPayment;
                    }
                    break;
                case 2:
                    {
                        orderStatus = OrderStatus.WaitingForReceiving;
                        extentOrderStatus = OrderStatus.WaitingForDelivery;
                    }
                    break;
                case 3:
                    {
                        orderStatus = OrderStatus.Completed;
                        evaluateStatus = EvaluateStatus.NotEvaluated;
                        refundStatus = OrderRefundStatus.NoRefund;
                    }
                    break;
                case 4:
                    {
                        refundStatus = OrderRefundStatus.Refunding;
                        extentRefundStatus = OrderRefundStatus.Refunded;
                    }
                    break;
                default:
                    break;
            }

            int totalCount;
            var orders = _orderService.LoadByPage(AuthorizedUser.Id, out totalCount, orderStatus, null, null, evaluateStatus, refundStatus, keywords, pageNo, limit, extentOrderStatus, extentRefundStatus).Select(o => new SimpleOderModel(o));
            var result = new ApiResult();
            var data = new
            {
                TotalCount = totalCount,
                Orders = orders
            };
            result.SetData(data);
            return result;
        }

        /// <summary>
        /// 订单详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [BasicAuthentication]
        public ApiResult Detail(Guid id)
        {
            var order = _orderService.Load(id);
            var result = new ApiResult();
            result.SetData(new SimpleOderModel(order));
            return result;
        }

    }
}
