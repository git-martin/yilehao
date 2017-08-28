using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BntWeb.Caching;
using BntWeb.Dashboard.Services;
using BntWeb.Logging;
using BntWeb.OrderProcess.Models;
using BntWeb.Security;
using BntWeb.Utility.Extensions;

namespace BntWeb.Dashboard.Controllers
{
    [AdminAuthorize]
    public class AdminController : Controller
    {
        private readonly IOrderStatisticsService _orderStattisticService;
        public AdminController(IOrderStatisticsService orderStattisticService)
        {
            _orderStattisticService = orderStattisticService;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        // GET: User
        public ActionResult Index()
        {
            ViewBag.SalesAmountToday = _orderStattisticService.SalesAmountToday();
            ViewBag.SalesVolumeToday = _orderStattisticService.SalesVolumeToday();
            ViewBag.NewMembersToday = _orderStattisticService.NewMembersToday();

            ViewBag.PendingPayment = _orderStattisticService.CountOrder(OrderStatus.PendingPayment);
            ViewBag.WaitingForDelivery = _orderStattisticService.CountOrder(OrderStatus.WaitingForDelivery);
            ViewBag.WaitingForReceiving = _orderStattisticService.CountOrder(OrderStatus.WaitingForReceiving);
            ViewBag.Refunding = _orderStattisticService.CountOrder(OrderRefundStatus.Refunding);
            ViewBag.Complated = _orderStattisticService.CountOrder(OrderStatus.Completed);

            ViewBag.InventoryWarning = _orderStattisticService.InventoryWarning();
            ViewBag.EvaluateUnDo = _orderStattisticService.CountUnDoEvalute();

            ViewBag.OnlyRefund = _orderStattisticService.CountRefundOrder(RefundType.OnlyRefund);
            ViewBag.RefundAndReturn = _orderStattisticService.CountRefundOrder(RefundType.RefundAndReturn);


            var statisticSalesResult = _orderStattisticService.StatisticSales();

            var ticks = new List<string>();
            var amount = new List<decimal>();
            var volume = new List<int>();

            foreach (var result in statisticSalesResult)
            {
                ticks.Add(result.Month);
                amount.Add(result.SalesAmount);
                volume.Add(result.SalesVolume);
            }

            ViewBag.StatisticSalesTicks = ticks.ToJson();
            ViewBag.StatisticSalesAmount = amount.ToJson();
            ViewBag.StatisticSalesVolume = volume.ToJson();


            var statisticMembersResult = _orderStattisticService.StatisticMembers();

            ViewBag.StatisticMembersTicks = statisticMembersResult.Select(m => m.Month).ToArray().ToJson();
            ViewBag.StatisticMembersAmount = statisticMembersResult.Select(m => m.Amount).ToArray().ToJson();

            ViewBag.DeliveryReminders = _orderStattisticService.DeliveryRemindersCount();
            return View();
        }
    }
}