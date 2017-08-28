using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BntWeb.Data.Services;
using BntWeb.Services;
using BntWeb.SinglePage.Services;
using BntWeb.WebApi.Filters;
using BntWeb.WebApi.Models;

namespace BntWeb.SinglePage.ApiControllers
{
    public class PageController : BaseApiController
    {
        private readonly ISinglePageService _singlePageService;
        public PageController(ISinglePageService singlePageService)
        {
            _singlePageService = singlePageService;
        }

        [HttpGet]
        public ApiResult Get(string key)
        {
            var result = new ApiResult();
            var page = _singlePageService.GetSinglePageByKey(key);
            result.SetData(page);
            return result;
        }
    }
}
