using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using BntWeb.Data;
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
    public class DeliveryReminderController : BaseApiController
    {
        private readonly ICurrencyService _currencyService;
        private readonly IOrderService _orderService;

        public DeliveryReminderController(ICurrencyService currencyService, IOrderService orderService)
        {
            _currencyService = currencyService;
            _orderService = orderService;
        }

        /// <summary>
        /// 提醒发货
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpPost]
        [BasicAuthentication]
        public ApiResult Remind(Guid orderId)
        {
            if (orderId.Equals(Guid.Empty))
                throw new WebApiInnerException("0001", "订单Id不合法");

            var order = _currencyService.GetSingleById<Order>(orderId);
            if (order == null)
                throw new WebApiInnerException("0002", "订单不存在");
            if (order.OrderStatus != OrderStatus.WaitingForDelivery)
                throw new WebApiInnerException("0003", "订单不是待发货状态，不能提醒发货");
            if (!order.MemberId.Equals(AuthorizedUser.Id))
                throw new WebApiInnerException("0004", "只能对自己的订单提醒发货");

            if(!_orderService.CheckTodayCanRemind(orderId,AuthorizedUser.Id))
                throw new WebApiInnerException("0005", "一天只能提醒发货一次");
            //var oldReminder = _currencyService.GetSingleByConditon<OrderDeliveryReminder>(x => x.OrderId == orderId && x.MemberId == AuthorizedUser.Id);
            //if (oldReminder != null && oldReminder.CreateTime.Subtract(DateTime.Now).Days == 0)
            //    throw new WebApiInnerException("0005", "一天只能提醒发货一次");

            var model = new OrderDeliveryReminder()
            {
                Id = KeyGenerator.GetGuidKey(),
                OrderId = orderId,
                OrderNo = order.OrderNo,
                MemberId = AuthorizedUser.Id,
                MemberName = AuthorizedUser.UserName,
                CreateTime = DateTime.Now
            };

            if (!_currencyService.Create(model))
            {
                throw new WebApiInnerException("0006", "创建发货提醒失败，内部错误");
            }

            ApiResult result = new ApiResult();
            return result;
        }


    }
}

