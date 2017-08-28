using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using BntWeb.Core.SystemSettings.Models;
using BntWeb.Data.Services;
using BntWeb.Validation;
using BntWeb.WebApi.Filters;
using BntWeb.WebApi.Models;

namespace BntWeb.Core.SystemSettings.ApiControllers
{
    public class DistrictController : BaseApiController
    {
        private readonly ICurrencyService _currencyService;

        public DistrictController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        [HttpGet]
        [BasicAuthentication]
        public ApiResult LoadByParentId(string id)
        {
            Argument.ThrowIfNullOrEmpty(id, "id参数不能为空");

            var result = new ApiResult();
            var districts =
                _currencyService.GetList<District>(d => d.ParentId.Equals(id, StringComparison.OrdinalIgnoreCase));

            result.SetData(districts);
            return result;
        }
    }
}
