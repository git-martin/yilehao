using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.Mvc;
using BntWeb.Security;
using BntWeb.Tag.ViewModels;

namespace BntWeb.Tag.Controllers
{
    public class AdminController : Controller
    {
        private readonly ICurrencyService _currencyService;

        public AdminController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        [AdminAuthorize]
        public ActionResult Index(string sourceType, string moduleKey)
        {
            return View();
        }

        [AdminAuthorize]
        public ActionResult Load(string sourceType, string moduleKey)
        {
            var result = new DataJsonResult();
            result.Data = _currencyService.GetList<Models.Tag>(t =>
                        t.SourceType.Equals(sourceType, StringComparison.OrdinalIgnoreCase) &&
                        t.ModuleKey.Equals(moduleKey, StringComparison.OrdinalIgnoreCase),
                        new OrderModelField { IsDesc = true, PropertyName = "Sort" });
            return Json(result);
        }

        [AdminAuthorize]
        public ActionResult Save(string sourceType, string moduleKey, EditTagViewModel editTag)
        {
            var result = new DataJsonResult();

            if (editTag.Id == Guid.Empty)
            {
                var tag = new Models.Tag
                {
                    Id = KeyGenerator.GetGuidKey(),
                    ModuleKey = moduleKey,
                    SourceType = sourceType,
                    Content = editTag.Content,
                    CreateTime = DateTime.Now,
                    Sort = editTag.Sort
                };

                _currencyService.Create(tag);
            }
            else
            {
                var tag = _currencyService.GetSingleById<Models.Tag>(editTag.Id);
                tag.Content = editTag.Content;
                tag.Sort = editTag.Sort;
                _currencyService.Update(tag);
            }

            return Json(result);
        }

        [AdminAuthorize]
        public ActionResult Delete(string sourceType, string moduleKey, Guid tagId)
        {
            var result = new DataJsonResult();
            _currencyService.DeleteByConditon<Models.Tag>(t => t.Id.Equals(tagId));
            return Json(result);
        }
    }
}