using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using BntWeb.Data.Services;
using BntWeb.FileSystems.Media;
using BntWeb.OrderProcess.Models;
using BntWeb.OrderProcess.Services;
using BntWeb.Utility.Extensions;
using BntWeb.Validation;
using BntWeb.WebApi.Filters;
using BntWeb.WebApi.Models;

namespace BntWeb.OrderProcess.ApiControllers
{
    public class OrderController : BaseApiController
    {
        private readonly IOrderService _orderService;
        private readonly ICurrencyService _currencyService;

        public OrderController(IOrderService orderService, ICurrencyService currencyService)
        {
            _orderService = orderService;
            _currencyService = currencyService;
        }

        /// <summary>
        /// 删除订单
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpDelete]
        [BasicAuthentication]
        public ApiResult Delete(Guid orderId)
        {
            if (orderId.Equals(Guid.Empty))
                throw new ArgumentNullException("订单Id");

            var order = _currencyService.GetSingleById<Order>(orderId);
            if (!order.MemberId.Equals(AuthorizedUser.Id))
                throw new WebApiInnerException("0001", "只能删除自己的订单");
            if (order.OrderStatus != OrderStatus.Closed && order.OrderStatus != OrderStatus.Completed)
                throw new WebApiInnerException("0002", "只能删除关闭或完成的订单");

            order.OrderStatus = OrderStatus.Deleted;
            if (!_currencyService.Update(order))
            {
                throw new WebApiInnerException("0003", "删除失败，可能订单已经被删除");
            }

            return new ApiResult();
        }

        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpPatch]
        [BasicAuthentication]
        public ApiResult Cancel(Guid orderId)
        {
            if (orderId.Equals(Guid.Empty))
                throw new ArgumentNullException("订单Id");

            var order = _currencyService.GetSingleById<Order>(orderId);
            if (!order.MemberId.Equals(AuthorizedUser.Id))
                throw new WebApiInnerException("0001", "只能处理自己的订单");

            if (order.OrderStatus != OrderStatus.PendingPayment)
                throw new WebApiInnerException("0002", "订单状态不合法，请联系商家取消订单");

            if (!_orderService.ChangeOrderStatus(orderId, OrderStatus.Closed, null, null, null, "客户取消订单"))
            {
                throw new WebApiInnerException("0003", "取消订单失败，可能订单状态已经变更");
            }

            return new ApiResult();
        }


        /// <summary>
        /// 确认收货
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpPatch]
        [BasicAuthentication]
        public ApiResult Complete(Guid orderId)
        {
            if (orderId.Equals(Guid.Empty))
                throw new ArgumentNullException("订单Id");

            var order = _currencyService.GetSingleById<Order>(orderId);
            if (!order.MemberId.Equals(AuthorizedUser.Id))
                throw new WebApiInnerException("0001", "只能处理自己的订单");

            if (order.OrderStatus != OrderStatus.WaitingForReceiving)
                throw new WebApiInnerException("0002", "订单状态不合法，无法确认收货");

            if (!_orderService.ChangeOrderStatus(orderId, OrderStatus.Completed, null, null, null, "客户确认收货"))
            {
                throw new WebApiInnerException("0003", "确认收货失败，可能订单状态已经变更");
            }

            return new ApiResult();
        }
    }
}
