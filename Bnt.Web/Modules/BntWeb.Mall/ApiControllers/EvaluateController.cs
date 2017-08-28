using System;
using System.Linq;
using System.Web.Http;
using BntWeb.Mall.ApiModels;
using BntWeb.Mall.Services;
using BntWeb.WebApi.Models;

namespace BntWeb.Mall.ApiControllers
{
    public class EvaluateController : BaseApiController
    {
        private readonly IGoodsService _goodsService;

        public EvaluateController(IGoodsService goodsService)
        {
            _goodsService = goodsService;
        }

        /// <summary>
        /// 获取商品评价列表分页
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="pageNo"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet]
        public ApiResult GetEvaluatesList(Guid goodsId, int pageNo = 1, int limit = 10)
        {
            int totalCount;
            var evaluates = _goodsService.GetGoodsEvaluatesListByPage(goodsId, pageNo, limit,out totalCount);
            ApiResult result = new ApiResult();
            var data = new
            {
                TotalCount=totalCount,
                Evaluates= evaluates.Select(x=>new GoodsEvaluateModel(x))
            };
            result.SetData(data);
            return result;
        }

        /// <summary>
        /// 获取商品最新九评价用户头像
        /// </summary>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        [HttpGet]
        public ApiResult GetNew9EvaluatesMember(Guid goodsId)
        {
            var evaluates = _goodsService.GetNew9Evaluates(goodsId);
            ApiResult result = new ApiResult();
            result.SetData(evaluates.Select(x=>new EvaluateMemberAvatarModel(x)));
            return result;
        }
    }
}
