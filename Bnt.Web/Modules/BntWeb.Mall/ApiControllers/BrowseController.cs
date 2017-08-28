using System;
using System.Linq;
using System.Web.Http;
using BntWeb.ContentMarkup.Models;
using BntWeb.ContentMarkup.Services;
using BntWeb.Mall.ApiModels;
using BntWeb.Mall.Services;
using BntWeb.WebApi.Filters;
using BntWeb.WebApi.Models;

namespace BntWeb.Mall.ApiControllers
{
    public class BrowseController : BaseApiController
    {
        private readonly IMarkupService _markupService;
        private readonly IGoodsService _goodsService;

        public BrowseController(IMarkupService markupService, IGoodsService goodsService)
        {
            _markupService = markupService;
            _goodsService = goodsService;
        }

        [HttpPost]
        [BasicAuthentication]
        public ApiResult CreateBrowse(Guid goodsId)
        {
            if (goodsId.Equals(Guid.Empty))
                throw new WebApiInnerException("0001", "商品Id不合法");
            if (_markupService.MarkupExist(goodsId, MallModule.Key, AuthorizedUser.Id, MarkupType.Browse))
                _markupService.CancelMarkup(goodsId, MallModule.Key, AuthorizedUser.Id, MarkupType.Browse);

            _markupService.CreateMarkup(goodsId, MallModule.Key, AuthorizedUser.Id, MarkupType.Browse);
            return new ApiResult();
        }

        [HttpDelete]
        [BasicAuthentication]
        public ApiResult ClearRecord()
        {
            _markupService.ClearMarkup(MallModule.Key, AuthorizedUser.Id, MarkupType.Browse);
            return new ApiResult();
        }

        [HttpGet]
        [BasicAuthentication]
        public ApiResult BrowseList(int pageNo = 1, int limit = 10)
        {
            int totalCount;
            var list = _goodsService.LoadBrowseGoodsByPage(AuthorizedUser.Id, pageNo, limit, out totalCount);

            var result = new ApiResult();
            var data = new
            {
                TotalCount = totalCount,
                Goods = list.Select(item => new BrowseListModel(item)).ToList()
            };
            result.SetData(data);
            return result;
        }
    }
}
