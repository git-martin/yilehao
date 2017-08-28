using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using BntWeb.Advertisement.Models;
using BntWeb.Advertisement.ViewModels;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.Environment;
using BntWeb.FileSystems.Media;
using BntWeb.Logging;
using BntWeb.Mvc;
using BntWeb.Security;
using BntWeb.Security.Identity;
using BntWeb.Utility.Extensions;
using BntWeb.Validation;
using BntWeb.Web.Extensions;

namespace BntWeb.Advertisement.Controllers
{
    public class AdminController : Controller
    {
        private readonly IStorageFileService _storageFileService;
        private readonly ICurrencyService _currencyService;
        public AdminController(IStorageFileService storageFileService, ICurrencyService currencyService)
        {
            _storageFileService = storageFileService;
            _currencyService = currencyService;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ViewAdvertisementKey })]
        public ActionResult AreaList()
        {
            ViewBag.Areas = _currencyService.GetAll<AdvertArea>().OrderBy(x => x.Key).ToList();

            return View();
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ViewAdvertisementKey })]
        public ActionResult AdvertList(Guid areaId)
        {
            var adverts = _currencyService.GetList<Advert>(a => a.AreaId.Equals(areaId), new OrderModelField { IsDesc = false, PropertyName = "Key" });
            foreach (var advert in adverts)
            {
                advert.AdvertImage = _storageFileService.GetFiles(advert.Id, AdvertisementModule.Key, "AdvertImage").FirstOrDefault();
            }
            ViewBag.Adverts = adverts;
            return View();
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ViewAdvertisementKey })]
        public ActionResult LoadAdvert(Guid areaId)
        {
            var resuslt = new DataJsonResult();
            resuslt.Data = _currencyService.GetList<Advert>(a => a.AreaId.Equals(areaId), new OrderModelField { IsDesc = false, PropertyName = "Key" });
            return Json(resuslt);
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.EditAdvertisementKey })]
        public ActionResult Send(Guid sourceId, string sourceTitle, string sourceType, string viewUrl, string moduleKey, string moduleName)
        {
            ViewBag.Areas = _currencyService.GetAll<AdvertArea>();

            var advert = new Advert
            {
                SourceId = sourceId,
                SourceTitle = sourceTitle,
                SourceType = sourceType,
                ViewUrl = viewUrl,
                ModuleKey = moduleKey,
                Description = sourceTitle,
                ModuleName = moduleName,
                ShotUrl = sourceType + "|" + sourceId
            };

            return View("Edit", advert);
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.EditAdvertisementKey })]
        public ActionResult Edit(Guid adverId)
        {
            ViewBag.Areas = _currencyService.GetAll<AdvertArea>();
            var advert = _currencyService.GetSingleById<Advert>(adverId);
            return View(advert);
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.EditAdvertisementKey })]
        public ActionResult EditOnPost(EditAdvertViewModel submitAdvert)
        {
            var result = new DataJsonResult();

            var advert = _currencyService.GetSingleById<Advert>(submitAdvert.Id);
            if (advert != null)
            {
                if (submitAdvert.SourceId != Guid.Empty)
                    advert.SourceId = submitAdvert.SourceId;

                if (!string.IsNullOrWhiteSpace(submitAdvert.ModuleKey))
                    advert.ModuleKey = submitAdvert.ModuleKey;

                if (!string.IsNullOrWhiteSpace(submitAdvert.ModuleName))
                    advert.ModuleName = submitAdvert.ModuleName;

                if (!string.IsNullOrWhiteSpace(submitAdvert.SourceType))
                    advert.SourceType = submitAdvert.SourceType;

                if (!string.IsNullOrWhiteSpace(submitAdvert.SourceTitle))
                    advert.SourceTitle = submitAdvert.SourceTitle;

                if (!string.IsNullOrWhiteSpace(submitAdvert.ViewUrl))
                    advert.ViewUrl = submitAdvert.ViewUrl;

                advert.Description = submitAdvert.Description;
                advert.ShotUrl = submitAdvert.ShotUrl;
                advert.LastUpdateTime = DateTime.Now;

                if (_currencyService.Update(advert))
                {
                    _storageFileService.ReplaceFile(advert.Id, AdvertisementModule.Key, AdvertisementModule.DisplayName, submitAdvert.AdvertImage, "AdvertImage");
                }
                else
                {
                    result.ErrorMessage = "保存发生未知错误，保存失败";
                }
            }
            else
            {
                result.ErrorMessage = "您所编辑的广告位不存在";
            }

            return Json(result);
        }
    }

}