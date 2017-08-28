using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BntWeb.Feedback.ApiModels;
using BntWeb.Feedback.Models;
using BntWeb.Feedback.Services;
using BntWeb.Utility.Extensions;
using BntWeb.Validation;
using BntWeb.WebApi.Filters;
using BntWeb.WebApi.Models;

namespace BntWeb.Feedback.ApiControllers
{
    public class FeedbackController : BaseApiController
    {
        private readonly IFeedbackService _feedbackService;
        private const string SourceTpye = "Common";
        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }


        /// <summary>
        /// 提交反馈 平台类型
        /// </summary>
        /// <param name="feedback"></param>
        /// <returns></returns>
        [HttpPost]
        [BasicAuthentication]
        public ApiResult Create([FromBody]CreateFeedbackModel feedback)
        {
            Argument.ThrowIfNullOrEmpty(feedback.Content, "反馈内容");

            var model = new Models.Feedback()
            {
                Content = feedback.Content,
                FeedbackType = feedback.FeedbackType,
                SourceType = SourceTpye,
                MemberId = AuthorizedUser.Id.ToGuid(),
                ModuleKey = FeedbackModule.Key,
                ModuleName = FeedbackModule.DisplayName
            };

            _feedbackService.CreateFeedback(model);
            
            ApiResult result = new ApiResult();
            return result;
        }

        /// <summary>
        /// 根据反馈类型获取用户反馈列表
        /// </summary>
        /// <param name="type"></param>
        /// <param name="pageNo"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet]
        [BasicAuthentication]
        public ApiResult GetListByType(FeedbackType type, int pageNo = 1, int limit = 10)
        {
            if (!Enum.IsDefined(typeof(FeedbackType), type))
                throw new WebApiInnerException("0001", "请求类型参数无效");

            int totalCount;
            var list = _feedbackService.GetFeedbackList(AuthorizedUser.Id.ToGuid(), type, SourceTpye, pageNo, limit, out totalCount);
            var result = new ApiResult();
            var data = new
            {
                TotalCount = totalCount,
                Feedbacks = list.Select(item => new FeedbackListModel(item)).ToList()
            };
            result.SetData(data);
            return result;
        }

    }
}
