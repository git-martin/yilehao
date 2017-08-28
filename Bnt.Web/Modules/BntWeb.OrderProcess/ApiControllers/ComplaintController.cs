using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using BntWeb.Data.Services;
using BntWeb.Feedback;
using BntWeb.Feedback.Models;
using BntWeb.Feedback.Services;
using BntWeb.FileSystems.Media;
using BntWeb.OrderProcess.ApiModels;
using BntWeb.OrderProcess.Models;
using BntWeb.Utility.Extensions;
using BntWeb.Validation;
using BntWeb.WebApi.Filters;
using BntWeb.WebApi.Models;

namespace BntWeb.OrderProcess.ApiControllers
{
    public class ComplaintController : BaseApiController
    {
        private readonly IFeedbackService _feedbackService;
        private readonly ICurrencyService _currencyService;
        private readonly IStorageFileService _storageFileService;

        public ComplaintController(IFeedbackService feedbackService, ICurrencyService currencyService, IStorageFileService storageFileService)
        {
            _feedbackService = feedbackService;
            _currencyService = currencyService;
            _storageFileService = storageFileService;
        }

        [HttpPost]
        [BasicAuthentication]
        public ApiResult Complaint([FromBody]CreateFeedbackModel feedback)
        {
            Argument.ThrowIfNullOrEmpty(feedback.Content, "投诉内容");
            if (feedback.OrderId.Equals(Guid.Empty))
                throw new WebApiInnerException("0001", "订单Id不合法");
            if (feedback.Content.Length > 900)
                throw new WebApiInnerException("0002", "投诉内容太长");

            var order = _currencyService.GetSingleById<Order>(feedback.OrderId);
            if (order == null)
                throw new WebApiInnerException("0003", "订单不存在");
            if (order.OrderStatus != OrderStatus.Completed)
                throw new WebApiInnerException("0004", "订单未完成，不可以投诉");
            if (!order.MemberId.Equals(AuthorizedUser.Id))
                throw new WebApiInnerException("0005", "只能投诉自己的订单");


            var model = new Feedback.Models.Feedback()
            {
                Content = $"单号：{order.OrderNo}，{feedback.Content}",
                FeedbackType = FeedbackType.Complaint,
                SourceId = feedback.OrderId,
                SourceType = "Order",
                MemberId = AuthorizedUser.Id.ToGuid(),
                ModuleKey = OrderProcessModule.Key,
                ModuleName = OrderProcessModule.DisplayName
            };

            _feedbackService.CreateFeedback(model, feedback.ImageFiles);

            ApiResult result = new ApiResult();
            return result;
        }


    }
}
