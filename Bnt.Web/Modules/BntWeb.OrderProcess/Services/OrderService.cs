/* 
    ======================================================================== 
        File name：        OrderService
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/7/6 16:20:23
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using System.Web;
using BntWeb.Config.Models;
using BntWeb.Data;
using BntWeb.FileSystems.Media;
using BntWeb.Logging;
using BntWeb.OrderProcess.ApiModels;
using BntWeb.OrderProcess.Models;
using BntWeb.OrderProcess.ViewModels;
using BntWeb.Security;
using BntWeb.Services;
using BntWeb.Utility.Extensions;
using BntWeb.Validation;
using BntWeb.Wallet.Services;

namespace BntWeb.OrderProcess.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUserContainer _userContainer;
        private readonly IStorageFileService _storageFileService;
        private readonly IEnumerable<IOrderServiceProxy> _orderServiceProxy;
        private readonly IWalletService _walletService;
        private readonly IConfigService _configService;

        public OrderService(IUserContainer userContainer, IStorageFileService storageFileService, IEnumerable<IOrderServiceProxy> orderServiceProxy, IWalletService walletService, IConfigService configService)
        {
            _userContainer = userContainer;
            _storageFileService = storageFileService;
            _orderServiceProxy = orderServiceProxy;
            _walletService = walletService;
            _configService = configService;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public void SubmitOrder(Order order, List<OrderGoods> orderGoods)
        {
            Argument.ThrowIfNullOrEmpty(order.ModuleKey, "ModuleKey");

            using (var dbContext = new OrderProcessDbContext())
            {
                if (order.Id.Equals(Guid.Empty))
                {
                    order.Id = KeyGenerator.GetGuidKey();
                    foreach (var goods in orderGoods)
                    {
                        goods.Id = KeyGenerator.GetGuidKey();
                        goods.OrderId = order.Id;
                    }
                }
                if (string.IsNullOrEmpty(order.OrderNo))
                    order.OrderNo = KeyGenerator.GetOrderNumber();

                dbContext.Orders.Add(order);
                dbContext.OrderGoods.AddRange(orderGoods);
                if (dbContext.SaveChanges() > 0)
                {
                    order.OrderGoods = orderGoods;
                    foreach (var proxy in _orderServiceProxy)
                    {
                        if (proxy.ModuleKey().Equals(order.ModuleKey, StringComparison.OrdinalIgnoreCase))
                            proxy.AfterSubmitOrder(order);
                    }
                    foreach (var goods in orderGoods)
                    {
                        if (goods.GoodsImage != null)
                            _storageFileService.AssociateFile(goods.Id, OrderProcessModule.Key, OrderProcessModule.DisplayName, goods.GoodsImage.Id, "GoodsImage");
                    }
                }
            }
        }

        public Order Load(Guid orderId)
        {
            using (var dbContext = new OrderProcessDbContext())
            {
                var order = dbContext.Orders.Where(o => o.Id.Equals(orderId)).Include(o => o.OrderActions).Include(o => o.OrderGoods).FirstOrDefault();

                if (order != null)
                {
                    foreach (var goods in order.OrderGoods)
                    {
                        goods.GoodsImage = _storageFileService.GetFiles(goods.Id, OrderProcessModule.Key, "GoodsImage").FirstOrDefault();
                    }
                }

                return order;
            }
        }

        /// <summary>
        /// 根据条件获取所有商品信息
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public List<Order> GetList(Expression<Func<Order, bool>> expression)
        {
            using (var dbContex = new OrderProcessDbContext())
            {
                var query = dbContex.Orders.Include(o=>o.OrderGoods).Where(expression).ToList();
                return query;
            }
        }

        public bool SetShippingInfo(Guid orderId, Guid shippingId, string shippingName, string shippingCode, string shippingNo)
        {
            using (var dbContext = new OrderProcessDbContext())
            {
                var order = dbContext.Orders.Include(o => o.OrderGoods).FirstOrDefault(o => o.Id.Equals(orderId));
                if (order != null && (!order.ShippingId.Equals(shippingId) || !order.ShippingNo.Equals(shippingNo, StringComparison.OrdinalIgnoreCase)))
                {
                    var memo = $"修改物流：{order.ShippingName}[{order.ShippingNo}]->{shippingName}[{shippingNo}]";
                    order.ShippingId = shippingId;
                    order.ShippingName = shippingName;
                    order.ShippingCode = shippingCode;
                    order.ShippingNo = shippingNo;
                    if (order.ShippingTime == null)
                        order.ShippingTime = DateTime.Now;
                    dbContext.Entry(order).State = EntityState.Modified;

                    var currentUser = _userContainer.CurrentUser;
                    dbContext.OrderActions.Add(new OrderAction
                    {
                        Id = KeyGenerator.GetGuidKey(),
                        OrderId = orderId,
                        Memo = memo,
                        CreateTime = DateTime.Now,
                        OrderStatus = order.OrderStatus,
                        PayStatus = order.PayStatus,
                        ShippingStatus = order.ShippingStatus,
                        EvaluateStatus = order.EvaluateStatus,
                        UserId = currentUser?.Id ?? Guid.Empty.ToString(),
                        UserName = currentUser?.UserName ?? "系统"
                    });
                    return dbContext.SaveChanges() > 0;
                }
                return false;
            }
        }

        public bool ChangeOrderStatus(Guid orderId, OrderStatus orderStatus, PayStatus? payStatus = null, ShippingStatus? shippingStatus = null, EvaluateStatus? evaluateStatus = null, string memo = "")
        {
            using (var dbContext = new OrderProcessDbContext())
            {
                var order = dbContext.Orders.Include(o => o.OrderGoods).FirstOrDefault(o => o.Id.Equals(orderId));
                if (order != null)
                {
                    var oldOrderStatus = order.OrderStatus;
                    var oldPayStatus = order.PayStatus;
                    var oldShippingStatus = order.ShippingStatus;
                    var oldEvaluateStatus = order.EvaluateStatus;
                    order.OrderStatus = orderStatus;
                    if (payStatus != null)
                        order.PayStatus = payStatus.Value;
                    if (shippingStatus != null)
                        order.ShippingStatus = shippingStatus.Value;
                    if (evaluateStatus != null)
                        order.EvaluateStatus = evaluateStatus.Value;

                    dbContext.Entry(order).State = EntityState.Modified;
                    var currentUser = _userContainer.CurrentUser;
                    dbContext.OrderActions.Add(new OrderAction
                    {
                        Id = KeyGenerator.GetGuidKey(),
                        OrderId = orderId,
                        Memo = memo,
                        CreateTime = DateTime.Now,
                        OrderStatus = order.OrderStatus,
                        PayStatus = order.PayStatus,
                        ShippingStatus = order.ShippingStatus,
                        EvaluateStatus = order.EvaluateStatus,
                        UserId = currentUser?.Id ?? Guid.Empty.ToString(),
                        UserName = currentUser?.UserName ?? "系统"
                    });

                    if (dbContext.SaveChanges() > 0)
                    {
                        foreach (var proxy in _orderServiceProxy)
                        {
                            if (proxy.ModuleKey().Equals(order.ModuleKey, StringComparison.OrdinalIgnoreCase))
                            {
                                proxy.AfterChangeOrderStatus(order, oldOrderStatus);
                                if (payStatus != null)
                                    proxy.AfterChangePayStatus(order, oldPayStatus);
                                if (shippingStatus != null)
                                    proxy.AfterChangeShippingStatus(order, oldShippingStatus);
                                if (evaluateStatus != null)
                                    proxy.AfterChangeEvaluateStatus(order, oldEvaluateStatus);
                            }
                        }
                        return true;
                    }
                }
                return false;
            }
        }

        public bool ChangePrice(Guid orderId, Guid orderGoodsId, decimal goodsPrice)
        {
            using (var dbContext = new OrderProcessDbContext())
            {
                var order = dbContext.Orders.FirstOrDefault(o => o.Id.Equals(orderId));
                if (order == null || order.OrderStatus != OrderStatus.PendingPayment)
                {
                    throw new Exception("订单状态不合法");
                }
                else
                {
                    var orderGoods = dbContext.OrderGoods.FirstOrDefault(og => og.Id.Equals(orderGoodsId));
                    if (orderGoods == null)
                    {
                        throw new Exception("订单商品未找到");
                    }
                    else
                    {
                        var oldGoodsPrice = orderGoods.Price;
                        orderGoods.Price = goodsPrice;
                        
                        //差价
                        var difference = (orderGoods.Price - oldGoodsPrice)* orderGoods.Quantity;

                        //计算商品总价
                        order.GoodsAmount = order.GoodsAmount + difference; //order.OrderGoods.Sum(g => g.Price * g.Quantity);
                        //计算订单总价
                        order.OrderAmount = order.GoodsAmount + order.ShippingFee;

                        //计算之前折抵的积分还值多少钱
                        var systemConfig = _configService.Get<SystemConfig>();
                        var integralMoney = (decimal)order.Integral / 100 * systemConfig.DiscountRate;

                        using (TransactionScope scope = new TransactionScope())
                        {
                            //如果是降价，并且抵扣的费用比商品总价高了，退还积分
                            string error = null;
                            if (oldGoodsPrice > orderGoods.Price && order.GoodsAmount < integralMoney)
                            {
                                //新费用需要多少积分
                                var integral = (int)(order.GoodsAmount / systemConfig.DiscountRate * 100);
                                //计算节省了多少积分
                                var refundIntegral = order.Integral - integral;
                                order.Integral = integral;
                                order.IntegralMoney = order.GoodsAmount;

                                _walletService.Deposit(order.MemberId, Wallet.Models.WalletType.Integral, refundIntegral, "修改订单产品价格，退还积分", out error);
                            }

                            if (string.IsNullOrWhiteSpace(error))
                            {
                                //计算需要支付的费用
                                order.PayFee = order.GoodsAmount - order.IntegralMoney + order.ShippingFee;
                                order.UnpayFee = order.PayFee;

                                dbContext.Entry(orderGoods).State = EntityState.Modified;
                                dbContext.Entry(order).State = EntityState.Modified;
                                Logger.Operation(
                                    $"{_userContainer.CurrentUser.UserName}修改了订单{order.OrderNo}中的商品【{orderGoods.GoodsName}】的价格为{orderGoods.Price}",
                                    OrderProcessModule.Instance, SecurityLevel.Danger);
                                if (dbContext.SaveChanges() > 0)
                                    scope.Complete();
                                return true;

                            }
                        }
                    }
                }
            }

            return false;
        }

        public List<Order> LoadByPage(string memberId, out int totalCount, OrderStatus? orderStatus = null, PayStatus? payStatus = null, ShippingStatus? shippingStatus = null, int pageIndex = 1, int pageSize = 10)
        {
            using (var dbContext = new OrderProcessDbContext())
            {
                totalCount = dbContext.Orders.Count();
                var orders =
                    dbContext.Orders.Where(o => o.MemberId.Equals(memberId, StringComparison.OrdinalIgnoreCase) && o.OrderStatus != OrderStatus.Deleted && (orderStatus == null || o.OrderStatus == orderStatus.Value) && (payStatus == null || o.PayStatus == payStatus.Value) && (shippingStatus == null || o.ShippingStatus == shippingStatus.Value)).Include(o => o.OrderGoods).OrderByDescending(o => o.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

                foreach (var order in orders)
                {
                    foreach (var goods in order.OrderGoods)
                    {
                        goods.GoodsImage =
                            _storageFileService.GetFiles(goods.Id, OrderProcessModule.Key, "GoodsImage")
                                .FirstOrDefault();
                    }
                }
                return orders;
            }
        }

        public List<Order> LoadByPage(string memberId, out int totalCount, OrderStatus? orderStatus = null, PayStatus? payStatus = null, ShippingStatus? shippingStatus = null, EvaluateStatus? evaluateStatus = null, OrderRefundStatus? refundStatus = null, string keywords = "", int pageIndex = 1, int pageSize = 10, OrderStatus? extentOrderStatus = null, OrderRefundStatus? extentRefundStatus = null)
        {
            using (var dbContext = new OrderProcessDbContext())
            {
                var noKeywords = string.IsNullOrWhiteSpace(keywords);
                var query =
                    dbContext.Orders.Where(o => o.MemberId.Equals(memberId, StringComparison.OrdinalIgnoreCase) && o.OrderStatus != OrderStatus.Deleted && (orderStatus == null || (o.OrderStatus == orderStatus.Value || (extentOrderStatus != null && o.OrderStatus == extentOrderStatus.Value))) && (payStatus == null || o.PayStatus == payStatus.Value) && (shippingStatus == null || o.ShippingStatus == shippingStatus.Value) && (evaluateStatus == null || o.EvaluateStatus == evaluateStatus.Value) && (refundStatus == null || (o.RefundStatus == refundStatus.Value || (extentRefundStatus != null && o.RefundStatus == extentRefundStatus.Value))) && (noKeywords || o.OrderNo.Contains(keywords))).Include(o => o.OrderGoods).OrderByDescending(o => o.CreateTime);

                totalCount = query.Count();
                var orders = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                foreach (var order in orders)
                {
                    foreach (var goods in order.OrderGoods)
                    {
                        goods.GoodsImage =
                            _storageFileService.GetFiles(goods.Id, OrderProcessModule.Key, "GoodsImage")
                                .FirstOrDefault();
                    }
                }
                return orders;
            }
        }
        public List<EvaluateDetailsModel> LoadOrderEvaluateList(Guid orderId)
        {
            using (var dbContext = new OrderProcessDbContext())
            {
                var query = from g in dbContext.OrderGoods
                            join e in dbContext.Evaluates on g.Id equals e.SourceId
                            where g.OrderId == orderId && e.SourceType.Equals("Order")
                            select new EvaluateDetailsModel
                            {
                                SingleGoodsId = g.SingleGoodsId,
                                GoodsId = g.GoodsId,
                                GoodsName = g.GoodsName,
                                GoodsAttribute = g.GoodsAttribute,
                                Price = g.Price,
                                Quantity = g.Quantity,
                                Score = e.Score,
                                Content = e.Content,
                                IsAnonymity = e.IsAnonymity,
                                MemberName = e.MemberName,
                                EvaluateTime = e.CreateTime,
                                ReplyContent = e.ReplyContent,
                                ReplyTime = e.ReplyTime
                            };

                return query.ToList();
            }
        }

        public List<OrderGoodsEvaluateViewModel> LoadOrderGoodsEvaluateList(Guid orderId)
        {
            using (var dbContext = new OrderProcessDbContext())
            {
                var query = from g in dbContext.OrderGoods
                            join e in dbContext.Evaluates on g.Id equals e.SourceId
                            where g.OrderId == orderId && e.SourceType.Equals("Order")
                            select new OrderGoodsEvaluateViewModel
                            {
                                OrderGoods = g,
                                Evaluate = e
                            };

                return query.ToList();
            }
        }

        public int CancelTimeOutOrder(DateTime outTime)
        {
            var count = 0;
            using (var dbContext = new OrderProcessDbContext())
            {
                var orders = dbContext.Orders.Where(o => o.OrderStatus == OrderStatus.PendingPayment && o.CreateTime < outTime).ToList();
                foreach (var order in orders)
                {
                    order.OrderStatus = OrderStatus.Closed;
                    if (ChangeOrderStatus(order.Id, order.OrderStatus))
                        count++;
                }
                return count;
            }
        }

        public int CompleteTimeOutOrder(DateTime outTime)
        {
            var count = 0;
            using (var dbContext = new OrderProcessDbContext())
            {
                var orders = dbContext.Orders.Where(o => o.OrderStatus == OrderStatus.WaitingForReceiving && o.ShippingTime < outTime).ToList();
                foreach (var order in orders)
                {
                    order.OrderStatus = OrderStatus.Completed;
                    if (ChangeOrderStatus(order.Id, order.OrderStatus))
                        count++;
                }
                return count;
            }
        }

        #region 提醒发货

        public OrderDeliveryReminder GetNewestReminderInfo(Guid orderId, string memberId)
        {
            using (var dbContext = new OrderProcessDbContext())
            {
                var reminderInfo = dbContext.OrderDeliveryReminders.Where(c => c.OrderId.Equals(orderId) && c.MemberId.Equals(memberId)).OrderByDescending(c=>c.CreateTime).FirstOrDefault();
                return reminderInfo;
            }
        }

        public bool CheckTodayCanRemind(Guid orderId, string memberId)
        {
            using (var dbContext = new OrderProcessDbContext())
            {
                var reminderInfo = dbContext.OrderDeliveryReminders.Where(c => c.OrderId.Equals(orderId) && c.MemberId.Equals(memberId)).OrderByDescending(c => c.CreateTime).FirstOrDefault();
                if (reminderInfo != null && reminderInfo.CreateTime.Subtract(DateTime.Now).Days == 0)
                    return false;
            }
            return true;
        }

        #endregion
    }
}