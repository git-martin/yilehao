/* 
    ======================================================================== 
        File name：        IOrderStattisticService
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/8/23 17:13:38
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using BntWeb.Config.Models;
using BntWeb.Mall;
using BntWeb.MemberBase;
using BntWeb.OrderProcess;
using BntWeb.OrderProcess.Models;
using BntWeb.Services;
using BntWeb.Utility.Extensions;

namespace BntWeb.Dashboard.Services
{
    public class OrderStatisticsService : IOrderStatisticsService
    {
        private readonly IConfigService _configService;

        public OrderStatisticsService(IConfigService configService)
        {
            _configService = configService;
        }

        public decimal SalesAmountToday()
        {
            using (var dbContext = new OrderProcessDbContext())
            {
                var startTime = DateTime.Now.DayZero();
                var endTime = DateTime.Now.DayEnd();

                return
                    dbContext.Orders.Where(
                        o =>
                            o.CreateTime >= startTime && o.CreateTime <= endTime &&
                            (o.OrderStatus == OrderStatus.WaitingForDelivery ||
                             o.OrderStatus == OrderStatus.WaitingForReceiving || o.OrderStatus == OrderStatus.Completed))
                        .Sum(o => (decimal?)o.PayFee) ?? 0;
            }
        }

        public int SalesVolumeToday()
        {
            using (var dbContext = new OrderProcessDbContext())
            {
                var startTime = DateTime.Now.DayZero();
                var endTime = DateTime.Now.DayEnd();

                return
                    dbContext.Orders.Count(
                        o =>
                            o.CreateTime >= startTime && o.CreateTime <= endTime &&
                            (o.OrderStatus == OrderStatus.WaitingForDelivery ||
                             o.OrderStatus == OrderStatus.WaitingForReceiving || o.OrderStatus == OrderStatus.Completed));
            }
        }

        public int NewMembersToday()
        {
            using (var dbContext = new MemberDbContext())
            {
                var startTime = DateTime.Now.DayZero();
                var endTime = DateTime.Now.DayEnd();
                return dbContext.Members.Count(m => m.CreateTime >= startTime && m.CreateTime <= endTime);
            }
        }

        public int CountOrder(OrderStatus orderStatus)
        {
            using (var dbContext = new OrderProcessDbContext())
            {
                return dbContext.Orders.Count(o => o.OrderStatus == orderStatus);
            }
        }

        public int CountOrder(OrderRefundStatus refundStatus)
        {
            using (var dbContext = new OrderProcessDbContext())
            {
                return dbContext.Orders.Count(o => o.RefundStatus == refundStatus);
            }
        }

        public int InventoryWarning()
        {
            var systemConfig = _configService.Get<SystemConfig>();

            using (var dbContext = new MallDbContext())
            {
                return dbContext.Goods.Count(o => o.Stock <= systemConfig.StockWarning && o.Status != Mall.Models.GoodsStatus.Delete);
            }
        }

        public List<StatisticSalesResult> StatisticSales()
        {
            using (var dbContext = new OrderProcessDbContext())
            {
                var startTime = DateTime.Now.AddMonths(-11).ToString("yyyy-MM-01").To<DateTime>();
                var query1 = from o in dbContext.Orders
                             where (o.OrderStatus == OrderStatus.Completed || o.OrderStatus == OrderStatus.WaitingForDelivery || o.OrderStatus == OrderStatus.WaitingForReceiving)
                             && o.CreateTime >= startTime && o.CreateTime <= DateTime.Now
                             select new
                             {
                                 o.PayFee,
                                 Month = o.CreateTime.Year + "-" + (o.CreateTime.Month<10?"0":"")+ o.CreateTime.Month
                             };

                var query = from t in query1
                            group t by t.Month into g
                            select new StatisticSalesResult
                            {
                                Month = g.Key,
                                SalesVolume = g.Count(),
                                SalesAmount = g.Sum(c => c.PayFee)
                            };

                return query.OrderBy(g => g.Month).ToList();
            }
        }

        public List<StatisticMembersResult> StatisticMembers()
        {
            using (var dbContext = new MemberDbContext())
            {
                var startTime = DateTime.Now.AddMonths(-11).ToString("yyyy-MM-01").To<DateTime>();
                var query1 = from o in dbContext.Members
                             where o.CreateTime >= startTime && o.CreateTime <= DateTime.Now
                             select new
                             {
                                 Month = o.CreateTime.Year + "-" + (o.CreateTime.Month < 10 ? "0" : "") + o.CreateTime.Month
                             };

                var query = from t in query1
                            group t by t.Month into g
                            select new StatisticMembersResult
                            {
                                Month = g.Key,
                                Amount = g.Count()
                            };

                return query.OrderBy(g => g.Month).ToList();
            }
        }

        public int DeliveryRemindersCount()
        {
            using (var dbContext = new OrderProcessDbContext())
            {
                return dbContext.OrderDeliveryReminders.Count();
            }
        }
        /// <summary>
        /// 统计退款数量，按类型统计
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public int CountRefundOrder(RefundType type)
        {
            using (var dbContext = new OrderProcessDbContext())
            {
                Expression<Func<ViewOrderRefund, bool>> expression =
                    l => l.OrderStatus != OrderStatus.Deleted &&
                         l.RefundStatus == RefundStatus.Applying && l.RefundType == type;

                return dbContext.ViewOrderRefunds.Where(expression).Count();

            }
        }
        /// <summary>
        /// 未处理评价数量
        /// </summary>
        /// <returns></returns>
        public int CountUnDoEvalute()
        {
            using (var dbContext = new OrderProcessDbContext())
            {
                return dbContext.EvaluateViews.Count(e => e.ReplyUserName == null);
            }
        }
    }
}