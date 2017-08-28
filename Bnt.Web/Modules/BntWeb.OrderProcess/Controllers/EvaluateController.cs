using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using BntWeb.Data.Services;
using BntWeb.Evaluate.Services;
using BntWeb.Mvc;
using BntWeb.OrderProcess.Models;
using BntWeb.OrderProcess.Services;
using BntWeb.OrderProcess.ViewModels;
using BntWeb.Security;
using BntWeb.Utility.Extensions;
using BntWeb.Web.Extensions;

namespace BntWeb.OrderProcess.Controllers
{
    public class EvaluateController : Controller
    {
        private readonly ICurrencyService _currencyService;
        private readonly IOrderService _orderService;
        private readonly IEvaluateService _evaluateService;

        public EvaluateController(ICurrencyService currencyService, IOrderService orderService, IEvaluateService evaluateService)
        {
            _currencyService = currencyService;
            _orderService = orderService;
            _evaluateService = evaluateService;
        }

        /// <summary>
        /// 订单评论详情
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [AdminAuthorize(PermissionsArray = new[] { Permissions.ViewOrderKey })]
        public ActionResult Detail(Guid orderId)
        {
            var order = _orderService.Load(orderId);
            var evaluates = _orderService.LoadOrderGoodsEvaluateList(orderId);

            ViewBag.EvaluateList = evaluates;
            return View(order);
        }

        /// <summary>
        /// 订单评论回复
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="replayList"></param>
        /// <returns></returns>
        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageEvaluateKey })]
        public ActionResult Replay(Guid orderId,List<EvaluateReplayViewModel> replayList)
        {
            List<Evaluate.Models.Evaluate> evaluates = new List<Evaluate.Models.Evaluate>();
            if (replayList != null)
            {
                foreach (var replay in replayList)
                {
                    evaluates.Add(new Evaluate.Models.Evaluate
                    {
                        Id = replay.Id,
                        ReplyContent = replay.ReplayContent
                    });
                }
            }
            _evaluateService.ReplayOrderEvaluates(evaluates, orderId,OrderProcessModule.Key);

            var result = new DataJsonResult();
            return Json(result);
        }

    }
}