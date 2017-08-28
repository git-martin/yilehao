using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using BntWeb.Advertisement.ApiModel;
using BntWeb.Advertisement.Models;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.FileSystems.Media;
using BntWeb.Security.Identity;
using BntWeb.Services;
using BntWeb.Utility.Extensions;
using BntWeb.Validation;
using BntWeb.WebApi.Filters;
using BntWeb.WebApi.Models;

namespace BntWeb.Advertisement.ApiControllers
{
    public class AdvertController : BaseApiController
    {
        private readonly IStorageFileService _storageFileService;
        private readonly ICurrencyService _currencyService;
        public AdvertController(IStorageFileService storageFileService, ICurrencyService currencyService)
        {
            _storageFileService = storageFileService;
            _currencyService = currencyService;
        }

        [HttpGet]
        public ApiResult Get(string key)
        {
            Argument.ThrowIfNullOrEmpty(key, "广告区域Key值");

            var advertArea =
                _currencyService.GetSingleByConditon<AdvertArea>(
                    a => a.Key.Equals(key, StringComparison.OrdinalIgnoreCase));

            if (advertArea == null)
                throw new WebApiInnerException("0001", "广告区域不存在");

            var result = new ApiResult();
            var adverts = _currencyService.GetList<Advert>(a => a.AreaId.Equals(advertArea.Id)).OrderBy(a => a.Key).Select(x=>new AdvertModel(x)).ToList();
            //foreach (var advert in adverts)
            //{
            //    advert.AdvertImage = _storageFileService.GetFiles(advert.Id, AdvertisementModule.Key, "AdvertImage").FirstOrDefault();
            //}
            //var data = adverts.Select(a => new
            //{
            //    a.Name,
            //    a.Key,
            //    Description = (a.Description ?? ""),
            //    AdvertImage = a.AdvertImage?.Simplified(),
            //    a.ShotUrl
            //});
            result.SetData(adverts);
            return result;
        }
    }
}
