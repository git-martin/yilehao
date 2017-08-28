using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using BntWeb.Data.Services;
using BntWeb.FileSystems.Media;
using BntWeb.OrderProcess.ApiModels;
using BntWeb.OrderProcess.Models;
using BntWeb.OrderProcess.Services;
using BntWeb.Utility.Extensions;
using BntWeb.Validation;
using BntWeb.WebApi.Filters;
using BntWeb.WebApi.Models;

namespace BntWeb.OrderProcess.ApiControllers
{
    public class RefundController : BaseApiController
    {
        private readonly ICurrencyService _currencyService;
        private readonly IRefundService _refundService;
        private readonly IStorageFileService _storageFileService;

        public RefundController(ICurrencyService currencyService, IRefundService refundService, IStorageFileService storageFileService)
        {
            _currencyService = currencyService;
            _refundService = refundService;
            _storageFileService = storageFileService;
        }

        /// <summary>
        /// 申请退款
        /// </summary>
        /// <param name="apply"></param>
        /// <returns></returns>
        [HttpPost]
        [BasicAuthentication]
        public ApiResult ApplyRefund([FromBody]RefundApplyModel apply)
        {
            Argument.ThrowIfNullOrEmpty(apply.Reason, "退款原因");
            if (apply.RefundAmount <=0)
                throw new WebApiInnerException("0009", "退款金额不能小于等于0");

            if (apply.OrderId.Equals(Guid.Empty))
                throw new WebApiInnerException("0001", "订单Id不合法");

            var order = _currencyService.GetSingleById<Order>(apply.OrderId);
            if (order == null)
                throw new WebApiInnerException("0002", "订单不存在");
            if (order.OrderStatus == OrderStatus.WaitingForDelivery && apply.RefundType == RefundType.RefundAndReturn)
                throw new WebApiInnerException("0003", "订单未发货，不能申请退款并退货");
            if (order.OrderStatus != OrderStatus.WaitingForDelivery && order.OrderStatus != OrderStatus.WaitingForReceiving)
                throw new WebApiInnerException("0004", "当前订单状态不能申请退款");
            if (!order.MemberId.Equals(AuthorizedUser.Id))
                throw new WebApiInnerException("0005", "只能对自己的订单申请退款");

            var singleGoods = _currencyService.GetSingleByConditon<OrderGoods>(x => x.OrderId == apply.OrderId && x.SingleGoodsId == apply.SingleGoodsId);
            if (singleGoods == null)
                throw new WebApiInnerException("0006", "商品不存在");
            if (singleGoods.RefundStatus != OrderRefundStatus.NoRefund)
                throw new WebApiInnerException("0007", "已申请退款不可重复申请");

            var maxRefundAmount = singleGoods.Price * singleGoods.Quantity;
            if (order.IntegralMoney > 0)
            {
                maxRefundAmount -= order.IntegralMoney * (singleGoods.Price * singleGoods.Quantity / order.GoodsAmount);
            }
            if (apply.RefundAmount > maxRefundAmount)
                throw new WebApiInnerException("0008", "退款金额不能大于单品实付金额");

            var model = new OrderRefund()
            {
                OrderId = apply.OrderId,
                SingleGoodsId = apply.SingleGoodsId,
                RefundType = apply.RefundType,
                RefundAmount = apply.RefundAmount,
                Reason = apply.Reason
            };

            _refundService.CreateOrderRefund(model);

            ApiResult result = new ApiResult();
            return result;
        }

        /// <summary>
        /// 修改退款申请
        /// </summary>
        /// <param name="apply"></param>
        /// <returns></returns>
        [HttpPatch]
        [BasicAuthentication]
        public ApiResult UpdateApply([FromBody]RefundApplyUpdateModel apply)
        {
            Argument.ThrowIfNullOrEmpty(apply.Reason, "退款原因");
            if (apply.RefundAmount <= 0)
                throw new WebApiInnerException("0009", "退款金额不能小于等于0");

            if (apply.Id.Equals(Guid.Empty))
                throw new WebApiInnerException("0001", "退款申请Id不合法");

            var oldApply = _currencyService.GetSingleById<OrderRefund>(apply.Id);
            if (oldApply == null)
                throw new WebApiInnerException("0002", "退款申请不存在");
            if (oldApply.RefundStatus == RefundStatus.Completed)
                throw new WebApiInnerException("0003", "退款已完成不得修改");
            var order = _currencyService.GetSingleById<Order>(oldApply.OrderId);
            if (order == null)
                throw new WebApiInnerException("0004", "订单不存在");
            if (order.OrderStatus == OrderStatus.WaitingForDelivery && apply.RefundType == RefundType.RefundAndReturn)
                throw new WebApiInnerException("0005", "订单未发货，不能申请退款并退货");
            if (order.OrderStatus != OrderStatus.WaitingForDelivery && order.OrderStatus != OrderStatus.WaitingForReceiving)
                throw new WebApiInnerException("0006", "当前订单状态不能申请退款");

            var singleGoods = _currencyService.GetSingleByConditon<OrderGoods>(x => x.OrderId == oldApply.OrderId && x.SingleGoodsId == oldApply.SingleGoodsId);
            if (singleGoods == null)
                throw new WebApiInnerException("0007", "商品不存在");

            var maxRefundAmount = singleGoods.Price * singleGoods.Quantity;
            if (order.IntegralMoney > 0)
            {
                maxRefundAmount += order.IntegralMoney * (singleGoods.Price * singleGoods.Quantity / order.GoodsAmount);
            }
            if (apply.RefundAmount > maxRefundAmount)
                throw new WebApiInnerException("0008", "退款金额不能大于单品实付金额");

            oldApply.RefundType = apply.RefundType;
            oldApply.RefundAmount = apply.RefundAmount;
            oldApply.Reason = apply.Reason;

            _currencyService.Update<Models.OrderRefund>(oldApply);

            ApiResult result = new ApiResult();
            return result;
        }

        /// <summary>
        /// 撤销退款申请
        /// </summary>
        /// <param name="applyId"></param>
        /// <returns></returns>
        [HttpDelete]
        [BasicAuthentication]
        public ApiResult RevokeApply(Guid applyId)
        {
            if (applyId.Equals(Guid.Empty))
                throw new WebApiInnerException("0001", "退款申请Id不合法");

            var refundApply = _currencyService.GetSingleById<OrderRefund>(applyId);
            if (refundApply == null)
                throw new WebApiInnerException("0002", "退款申请不存在");

            if (refundApply.RefundStatus != RefundStatus.Applying)
                throw new WebApiInnerException("0003", "退款申请已处理不得撤销");

            _refundService.RevokeOrderRefund(refundApply);

            ApiResult result = new ApiResult();
            return result;
        }

        /// <summary>
        /// 获取订单商品退款详情
        /// </summary>
        /// <param name="orderId">订单id</param>
        /// <param name="singleGoodsId">单品Id</param>
        /// <returns></returns>
        [HttpGet]
        [BasicAuthentication]
        public ApiResult ApplyInfo(Guid orderId, Guid singleGoodsId)
        {
            var apply =
                _currencyService.GetList<OrderRefund>(
                    x => x.OrderId == orderId && x.SingleGoodsId == singleGoodsId).OrderByDescending(x => x.CreateTime).Select(o => new RefundApplyInfoModel(o)).FirstOrDefault();
            if (apply == null)
                throw new WebApiInnerException("0001", "当前订单商品无退款申请");

            var orderGoods = _currencyService.GetSingleByConditon<OrderGoods>(x=> x.OrderId == orderId && x.SingleGoodsId == singleGoodsId);
            OrderSingleGoodsModel singleGoods = new OrderSingleGoodsModel();
            if (orderGoods != null)
            {
                singleGoods.GoodsId = orderGoods.GoodsId;
                singleGoods.SingleGoodsId = orderGoods.SingleGoodsId;
                singleGoods.GoodsAttribute = orderGoods.GoodsAttribute;
                singleGoods.GoodsName = orderGoods.GoodsName;
                singleGoods.Price = orderGoods.Price;
                singleGoods.Quantity = orderGoods.Quantity;

                var goodsImage = _storageFileService.GetFiles(orderGoods.Id, OrderProcessModule.Key, "GoodsImage").FirstOrDefault();
                singleGoods.MainImage = goodsImage?.Simplified();
            }
            ApiResult result = new ApiResult();
            var data = new
            {
                Datail = apply,
                Goods = singleGoods
            };
            result.SetData(data);
            return result;
        }
    }
}

