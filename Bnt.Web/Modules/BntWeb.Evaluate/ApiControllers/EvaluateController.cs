using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using BntWeb.Data;
using BntWeb.Evaluate.ApiModels;
using BntWeb.Evaluate.Services;
using BntWeb.MemberBase.Services;
using BntWeb.Utility.Extensions;
using BntWeb.Validation;
using BntWeb.WebApi.Filters;
using BntWeb.WebApi.Models;

namespace BntWeb.Evaluate.ApiControllers
{
    /// <summary>
    /// 评价Api根据需要复制到对应模块并做对应修改。注意：接口没有测试，仅供参考。
    /// </summary>
    public class EvaluateController : BaseApiController
    {
        private readonly IEvaluateService _evaluateService;
        private readonly IMemberService _memberService;
        private const string SourceType = "*********";
        private const string FileTypeName = "*********";
        public EvaluateController(IEvaluateService evaluateService, IMemberService memberService)
        {
            _evaluateService = evaluateService;
            _memberService = memberService;
        }

        /// <summary>
        /// 获取评价列表
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="pageNo"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet]
        public ApiResult List(string sourceId, int pageNo = 1, int limit = 10)
        {
            Argument.ThrowIfNullOrEmpty(sourceId, "评价Id");
            var id = sourceId.ToGuid();
            if (id.Equals(Guid.Empty))
                throw new WebApiInnerException("0001", "评价Id格式不正确");
            int totalCount;
            var evaluates = _evaluateService.LoadByPage(id, EvaluateModule.Instance, SourceType, out totalCount, pageNo, limit);

           

            var list = evaluates.Select(evaluate => new ListEvaluateModel(evaluate)).ToList();

            var result = new ApiResult();
            var data = new
            {
                TotalCount = totalCount,
                Evaluates = list
            };
            result.SetData(data);
            return result;
        }

        /// <summary>
        /// 发布评论
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="postEvaluate"></param>
        /// <returns></returns>
        [HttpPost]
        [BasicAuthentication]
        public ApiResult Create(string sourceId, [FromBody] CreateEvaluateModel postEvaluate)
        {
            Argument.ThrowIfNullOrEmpty(postEvaluate.Content, "评论内容");
            Argument.ThrowIfNullOrEmpty(sourceId, "话题Id");
            var id = sourceId.ToGuid();
            if (id.Equals(Guid.Empty))
                throw new WebApiInnerException("0001", "话题Id类型格式不正确");

            if (postEvaluate.Content.Length > 1000)
                throw new WebApiInnerException("0002", "评论内容太长");


            var member = _memberService.FindMemberById(AuthorizedUser.Id);
            var evaluate = new Models.Evaluate()
            {
                Id = KeyGenerator.GetGuidKey(),
                Content = postEvaluate.Content,
                Score = postEvaluate.Score,
                MemberId = AuthorizedUser.Id,
                MemberName = member.NickName,
                SourceId = id,
                ModuleKey = EvaluateModule.Key,
                ModuleName = EvaluateModule.DisplayName,
                SourceType = SourceType,
                CreateTime = DateTime.Now,
                IsAnonymity = false,
                Status = 1,
            };

            _evaluateService.SaveEvaluate(evaluate);
            var result = new ApiResult();
            result.SetData(new ListEvaluateModel(evaluate));
            return result;
        }

        /// <summary>
        /// 删除评论
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [BasicAuthentication]
        public ApiResult Delete(string sourceId, string id)
        {
            Argument.ThrowIfNullOrEmpty(id, "Id不能为空");

            var result = new ApiResult();
            var evaluateId = id.ToGuid();
            if (evaluateId.Equals(Guid.Empty))
                throw new WebApiInnerException("0001", "评论id格式不正确");

            var evaluate = _evaluateService.GetEvaluate(evaluateId);
            if (evaluate != null && evaluate.MemberId.Equals(AuthorizedUser.Id, StringComparison.OrdinalIgnoreCase))
                _evaluateService.DeleteEvaluate(evaluateId);
            else
                throw new WebApiInnerException("0002", "只可以删除自己的评论");

            return result;
        }
    }
}
