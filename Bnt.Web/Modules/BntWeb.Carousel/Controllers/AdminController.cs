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
using BntWeb.Carousel.Models;
using BntWeb.Carousel.Services;
using BntWeb.Carousel.ViewModels;
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

namespace BntWeb.Carousel.Controllers
{
    public class AdminController : Controller
    {
        private readonly ICarouselService _carouselService;

        public AdminController(ICarouselService carouselService)
        {
            _carouselService = carouselService;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ViewCarouselKey })]
        public ActionResult GroupList()
        {
            ViewBag.Groups = _carouselService.LoadAllGroups();

            return View();
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ViewCarouselKey })]
        public ActionResult ItemList(Guid groupId)
        {
            ViewBag.GroupId = groupId;
            ViewBag.Items = _carouselService.LoadItemsByGroupId(groupId);
            return View();
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.EditCarouselKey })]
        public ActionResult Add(Guid sourceId, string sourceTitle, string sourceType, string viewUrl, string moduleKey, string moduleName)
        {
            var groups = _carouselService.LoadAllGroups();

            ViewBag.Groups = groups;
            ViewBag.SelectedGroup = groups.FirstOrDefault()?.Id;

            var carousel = new CarouselItem
            {
                SourceId = sourceId,
                SourceTitle = sourceTitle,
                SourceType = sourceType,
                ViewUrl = viewUrl,
                ModuleKey = moduleKey,
                ModuleName = moduleName,
                ShotUrl = sourceType + "|" + sourceId
            };

            return View("Edit", carousel);
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.EditCarouselKey })]
        public ActionResult AddWithGroupId(Guid groupId)
        {
            ViewBag.Groups = _carouselService.LoadAllGroups();
            ViewBag.SelectedGroup = groupId;

            var carousel = new CarouselItem();
            carousel.GroupId = groupId;
            carousel.ModuleKey = CarouselModule.Key;
            carousel.ModuleName = CarouselModule.DisplayName;
            carousel.SourceType = "Custom";

            return View("Edit", carousel);
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.EditCarouselKey })]
        public ActionResult Edit(Guid itemId)
        {
            ViewBag.Groups = _carouselService.LoadAllGroups();

            var carousel = _carouselService.LoadItemById(itemId);
            ViewBag.SelectedGroup = carousel.GroupId;

            return View("Edit", carousel);
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.EditCarouselKey })]
        public ActionResult EditOnPost([FromBody]EditCarouselItemViewModel submitCarousel)
        {
            var result = new DataJsonResult();
            if (submitCarousel.CoverImage == Guid.Empty)
                result.ErrorMessage = "还没有上传轮播图";
            else
            {
                try
                {
                    CarouselItem carousel = null;
                    if (submitCarousel.Id != Guid.Empty)
                        carousel = _carouselService.LoadItemById(submitCarousel.Id);
                    bool isNew = carousel == null;
                    if (isNew)
                    {
                        carousel = new CarouselItem
                        {
                            Id = KeyGenerator.GetGuidKey(),
                            ModuleKey = submitCarousel.ModuleKey,
                            ModuleName = submitCarousel.ModuleName,
                            ViewUrl = submitCarousel.ViewUrl,
                            SourceType = submitCarousel.SourceType,
                            CreateTime = DateTime.Now
                        };

                        if (submitCarousel.SourceId != Guid.Empty)
                            carousel.SourceId = submitCarousel.SourceId;
                    }

                    carousel.GroupId = submitCarousel.GroupId;
                    carousel.SourceTitle = submitCarousel.SourceTitle;
                    carousel.Sort = submitCarousel.Sort;
                    carousel.ShotUrl = submitCarousel.ShotUrl;
                    carousel.Summary = submitCarousel.Summary;
                    carousel.CoverImage = new StorageFile { Id = submitCarousel.CoverImage };

                    if (!_carouselService.SaveCarouselItem(carousel))
                    {
                        result.ErrorMessage = "保存轮播项出现意外错误";
                    }

                }
                catch (Exception ex)
                {
                    result.ErrorMessage = "保存轮播项出现意外错误";
                    Logger.Error(ex, "保存轮播项出错了");
                }
            }

            return Json(result);
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.DeleteCarouselKey })]
        public ActionResult Delete(Guid id)
        {

            var result = new DataJsonResult();

            if (!_carouselService.DeleteCarouselItem(id))
            {
                result.ErrorMessage = "删除发生错误，删除失败";
            }

            return Json(result);
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.EditCarouselKey })]
        public ActionResult SetSort(Guid id, int sort)
        {

            var result = new DataJsonResult();
            var carousel = _carouselService.LoadItemById(id);
            if (carousel != null)
            {
                carousel.Sort = sort;
                if (!_carouselService.SaveCarouselItem(carousel))
                    result.ErrorMessage = "设置排序发生错误，设置失败";
            }

            return Json(result);
        }
    }

}