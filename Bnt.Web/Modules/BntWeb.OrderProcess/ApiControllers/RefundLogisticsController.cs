using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BntWeb.Data.Services;
using BntWeb.OrderProcess.ApiModels;
using BntWeb.OrderProcess.Models;
using BntWeb.Validation;
using BntWeb.WebApi.Filters;
using BntWeb.WebApi.Models;

namespace BntWeb.OrderProcess.ApiControllers
{
    public class RefundLogisticsController : BaseApiController
    {
        private readonly ICurrencyService _currencyService;

        public RefundLogisticsController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        /// <summary>
        /// 退款申请 添加物流信息
        /// </summary>
        /// <param name="apply"></param>
        /// <returns></returns>
        [HttpPost]
        [BasicAuthentication]
        public ApiResult ApplyRefund([FromBody]RefundLogisticsModel apply)
        {
            Argument.ThrowIfNullOrEmpty(apply.ShippingName, "物流名称");
            Argument.ThrowIfNullOrEmpty(apply.ShippingNo, "物流编号");
            Argument.ThrowIfNullOrEmpty(apply.ShippingMemo, "物流说明");

            if (apply.Id.Equals(Guid.Empty))
                throw new WebApiInnerException("0001", "退款申请Id不合法");

            var oldApply = _currencyService.GetSingleById<OrderRefund>(apply.Id);
            if (oldApply == null)
                throw new WebApiInnerException("0002", "退款申请不存在");
            if (!(oldApply.RefundType == RefundType.RefundAndReturn && oldApply.RefundStatus == RefundStatus.Processed && oldApply.ReviewResult == ReviewResult.Passed))
                throw new WebApiInnerException("0003", "当前退款状态不能添加物流信息");
            if(oldApply.ShippingTime!=null)
                throw new WebApiInnerException("0004", "物流信息已提交不得重复提交");
            oldApply.ShippingName = apply.ShippingName;
            oldApply.ShippingNo = apply.ShippingNo;
            oldApply.ShippingMemo = apply.ShippingMemo;
            oldApply.ShippingTime = DateTime.Now;
            _currencyService.Update<OrderRefund>(oldApply);

            ApiResult result = new ApiResult();
            return result;
        }

    }
}
