using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Autofac;
using BntWeb.Core.SystemSettings.Models;
using BntWeb.Data.Services;
using BntWeb.Environment;
using BntWeb.Environment.Configuration;
using BntWeb.FileSystems.Media;
using BntWeb.Logging;
using BntWeb.Validation;
using BntWeb.WebApi.Filters;
using BntWeb.WebApi.Models;

namespace BntWeb.Core.SystemSettings.ApiControllers
{
    public class ReleaseController : BaseApiController
    {
        private readonly ICurrencyService _currencyService;
        private readonly IAppConfigurationAccessor _appConfigurationAccessor;

        public ReleaseController(ICurrencyService currencyService, IAppConfigurationAccessor appConfigurationAccessor)
        {
            _currencyService = currencyService;
            _appConfigurationAccessor = appConfigurationAccessor;
        }

        [HttpGet]
        public ApiResult GetLastVersion(string softKey)
        {
            int totalCount;
            var last = _currencyService.GetListPaged<SoftRelease>(1, 1, sr => sr.SoftKey.Equals(softKey, StringComparison.OrdinalIgnoreCase), out totalCount, new OrderModelField { IsDesc = true, PropertyName = "CreateTime" });
            var result = new ApiResult();
            if (last.Count > 0)
            {
                var item = last.FirstOrDefault();
                if (item != null)
                {
                    var fileHttpUrl = _appConfigurationAccessor.GetConfiguration("FileHttpUrl");
                    item.DownloadUrl = fileHttpUrl + item.DownloadUrl;
                    result.SetData(item);
                }
            }
            return result;
        }
    }
}
