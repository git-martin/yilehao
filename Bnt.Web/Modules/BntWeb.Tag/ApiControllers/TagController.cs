/* 
    ======================================================================== 
        File name：		TagController
        Module:			
        Author：		        罗嗣宝
        Create Time：		2016/6/22 10:19:21
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Http;
using BntWeb.Data.Services;
using BntWeb.FileSystems.Media;
using BntWeb.Utility.Extensions;
using BntWeb.Validation;
using BntWeb.WebApi.Filters;
using BntWeb.WebApi.Models;

namespace BntWeb.Tag.ApiControllers
{
    public class TagController : BaseApiController
    {
        private readonly ICurrencyService _currencyService;

        public TagController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        [HttpGet]
        public ApiResult Get(string sourceType, string moduleKey)
        {
            Argument.ThrowIfNullOrEmpty(sourceType, "数据源类型");
            Argument.ThrowIfNullOrEmpty(moduleKey, "模块Key");

            var result = new ApiResult();
            var data = _currencyService.GetList<Models.Tag>(t =>
                        t.SourceType.Equals(sourceType, StringComparison.OrdinalIgnoreCase) &&
                        t.ModuleKey.Equals(moduleKey, StringComparison.OrdinalIgnoreCase),
                        new OrderModelField { IsDesc = true, PropertyName = "Sort" }).Select(t=>t.Content).ToArray();
            result.SetData(data);
            return result;
        }

    }
}