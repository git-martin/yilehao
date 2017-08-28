using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Autofac;
using BntWeb.Data.Services;
using BntWeb.Environment;
using BntWeb.Evaluate.Services;
using BntWeb.FileSystems.Media;
using BntWeb.OrderProcess.ApiModels;
using BntWeb.OrderProcess.Models;
using BntWeb.OrderProcess.Services;
using BntWeb.Validation;
using BntWeb.WebApi.Filters;
using BntWeb.WebApi.Models;
using BntWeb.MemberBase.Models;

namespace BntWeb.OrderProcess.ApiControllers
{
    public class EvaluateController : BaseApiController
    {
        private readonly IEvaluateService _evaluateService;
        private readonly ICurrencyService _currencyService;
        private readonly IOrderService _orderService;
        private readonly IStorageFileService _storageFileService;

        public EvaluateController(IEvaluateService evaluateService, ICurrencyService currencyService, IOrderService orderService, IStorageFileService storageFileService)
        {
            _evaluateService = evaluateService;
            _currencyService = currencyService;
            _orderService = orderService;
            _storageFileService = storageFileService;
        }

        [HttpPost]
        [BasicAuthentication]
        public ApiResult SubmitEvaluate(Guid orderId, [FromBody]CreateEvaluateListModel evaluates)
        {
            if (orderId.Equals(Guid.Empty))
                throw new WebApiInnerException("0001", "订单Id不合法");
            if (evaluates.Evaluates.Count == 0)
                throw new WebApiInnerException("0002", "评价内容不能为空");

            var order = _currencyService.GetSingleById<Order>(orderId);
            if (order == null)
                throw new WebApiInnerException("0003", "订单不存在");
            if (order.OrderStatus != OrderStatus.Completed)
                throw new WebApiInnerException("0004", "订单未完成，不可以评价");
            if (!order.MemberId.Equals(AuthorizedUser.Id))
                throw new WebApiInnerException("0005", "只能对自己的订单进行评价");

            List<Evaluate.Models.Evaluate> evaluateList = new List<Evaluate.Models.Evaluate>();
            foreach (var evaluateInfo in evaluates.Evaluates)
            {
                if (evaluateInfo.Score > 5 || evaluateInfo.Score < 0)
                    throw new WebApiInnerException("0006", "商品评分不能大于5且不能小于0");

                if (evaluateInfo.SingleGoodsId.Equals(Guid.Empty))
                    throw new WebApiInnerException("0007", "单品Id不合法");
                Argument.ThrowIfNullOrEmpty(evaluateInfo.Content, "评价内容");

                var goods =
                    _currencyService.GetSingleByConditon<OrderGoods>(
                        x => x.OrderId == orderId && x.SingleGoodsId == evaluateInfo.SingleGoodsId);
                if (goods == null)
                    throw new WebApiInnerException("0008", "单品不存在");
                var refund = _currencyService.Count<OrderRefund>(
                        x => x.OrderId == orderId && x.SingleGoodsId == evaluateInfo.SingleGoodsId && x.RefundStatus == RefundStatus.Completed && x.ReviewResult == ReviewResult.Passed);
                if (refund > 0)
                    throw new WebApiInnerException("0009", "单品已退款不能评价");

                var member = _currencyService.GetSingleById<Member>(AuthorizedUser.Id);
                var model = new Evaluate.Models.Evaluate()
                {
                    Score = evaluateInfo.Score,
                    Content = evaluateInfo.Content,
                    SourceId = goods.Id,
                    ExtentsionId = evaluateInfo.SingleGoodsId,
                    SourceType = "Order",
                    MemberId = AuthorizedUser.Id,
                    MemberName = member?.NickName ?? (member?.UserName),
                    ModuleKey = OrderProcessModule.Key,
                    ModuleName = OrderProcessModule.DisplayName,
                    IsAnonymity = evaluateInfo.IsAnonymity,
                    FilesId = evaluateInfo.FilesId
                };

                evaluateList.Add(model);
            }

            var orderGoods = _currencyService.Count<OrderGoods>(x => x.OrderId == orderId);
            var orderRefunds = _currencyService.Count<OrderRefund>(
                        x => x.OrderId == orderId && x.RefundStatus == RefundStatus.Completed && x.ReviewResult == ReviewResult.Passed);
            if (orderGoods - orderRefunds != evaluateList.Count)
                throw new WebApiInnerException("0010", "订单需评价商品数与提交评价数不符");

            _evaluateService.CreateOrderEvaluates(evaluateList, order.Id);
            ApiResult result = new ApiResult();
            return result;
        }


        [HttpGet]
        [BasicAuthentication]
        public ApiResult Detail(Guid orderId)
        {
            var evaluateList = _orderService.LoadOrderEvaluateList(orderId);
            foreach (var evaluate in evaluateList)
            {
                var goods = _currencyService.GetSingleByConditon<OrderGoods>(x => x.OrderId == orderId && x.SingleGoodsId == evaluate.SingleGoodsId);
                if (goods != null)
                {
                    var goodsImage = _storageFileService.GetFiles(goods.Id, OrderProcessModule.Key, "GoodsImage").FirstOrDefault();
                    evaluate.MainImage = goodsImage?.Simplified();
                }
            }

            ApiResult result = new ApiResult();
            result.SetData(evaluateList);
            return result;
        }
    }
}
