using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.Evaluate;
using BntWeb.Evaluate.Services;
using BntWeb.Logging;
using BntWeb.OrderProcess.Models;
using BntWeb.Security;

namespace BntWeb.OrderProcess.Services
{
    public class EvaluateServiceProxy: IEvaluateServiceProxy
    {
        private readonly IUserContainer _userContainer;
        private readonly ICurrencyService _currencyService;
        private readonly IEnumerable<IOrderServiceProxy> _orderServiceProxy;

        public EvaluateServiceProxy(IUserContainer userContainer, ICurrencyService currencyService, IEnumerable<IOrderServiceProxy> orderServiceProxy)
        {
            _userContainer = userContainer;
            _currencyService = currencyService;
            _orderServiceProxy = orderServiceProxy;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public string ModuleKey()
        {
            return OrderProcessModule.Key;
        }

        /// <summary>
        /// 订单评价后修改订单评价状态
        /// </summary>
        /// <param name="orderId"></param>
        public void AfterCreateOrderEvaluates(Guid orderId)
        {
            using (var dbContext = new OrderProcessDbContext())
            {
                var order = dbContext.Orders.FirstOrDefault(x=>x.Id==orderId);
                if (order != null)
                {
                    var oldEvaluateStatus = order.EvaluateStatus;
                    order.EvaluateStatus = EvaluateStatus.Evaluated;
                    dbContext.Entry(order).State = System.Data.Entity.EntityState.Modified;

                    var currentUser = _userContainer.CurrentUser;
                    dbContext.OrderActions.Add(new OrderAction
                    {
                        Id = KeyGenerator.GetGuidKey(),
                        OrderId = order.Id,
                        Memo = "客户评价",
                        CreateTime = DateTime.Now,
                        OrderStatus = order.OrderStatus,
                        PayStatus = order.PayStatus,
                        ShippingStatus = order.ShippingStatus,
                        EvaluateStatus = order.EvaluateStatus,
                        RefundStatus = order.RefundStatus,
                        UserId = currentUser.Id,
                        UserName = currentUser.UserName
                    });

                    if (dbContext.SaveChanges() > 0)
                    {
                        Logger.Operation($"订单评价{order.OrderNo}，修改订单评价状态为已评价", OrderProcessModule.Instance);

                        foreach (var proxy in _orderServiceProxy)
                        {
                            if (proxy.ModuleKey().Equals(order.ModuleKey, StringComparison.OrdinalIgnoreCase))
                            {
                                proxy.AfterChangeEvaluateStatus(order, oldEvaluateStatus);
                            }
                        }
                    }
                }
            }
        }

        public void AfterReplayOrderEvaluates(Guid orderId)
        {
            using (var dbContext = new OrderProcessDbContext())
            {
                var order = dbContext.Orders.FirstOrDefault(x => x.Id == orderId);
                if (order != null)
                {
                    var oldEvaluateStatus = order.EvaluateStatus;
                    order.EvaluateStatus = EvaluateStatus.Replied;
                    dbContext.Entry(order).State = System.Data.Entity.EntityState.Modified;

                    var currentUser = _userContainer.CurrentUser;
                    dbContext.OrderActions.Add(new OrderAction
                    {
                        Id = KeyGenerator.GetGuidKey(),
                        OrderId = order.Id,
                        Memo = "评价回复",
                        CreateTime = DateTime.Now,
                        OrderStatus = order.OrderStatus,
                        PayStatus = order.PayStatus,
                        ShippingStatus = order.ShippingStatus,
                        EvaluateStatus = order.EvaluateStatus,
                        RefundStatus = order.RefundStatus,
                        UserId = currentUser.Id,
                        UserName = currentUser.UserName
                    });

                    if (dbContext.SaveChanges() > 0)
                    {
                        Logger.Operation($"订单{order.OrderNo}评价回复，修改订单评价状态为已回复", OrderProcessModule.Instance);

                        foreach (var proxy in _orderServiceProxy)
                        {
                            if (proxy.ModuleKey().Equals(order.ModuleKey, StringComparison.OrdinalIgnoreCase))
                            {
                                proxy.AfterChangeEvaluateStatus(order, oldEvaluateStatus);
                            }
                        }
                    }
                }
            }
        }

    }
}