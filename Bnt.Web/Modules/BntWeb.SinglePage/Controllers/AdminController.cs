using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.FileSystems.Media;
using BntWeb.Mvc;
using BntWeb.Security;
using BntWeb.SinglePage.Services;
using BntWeb.SinglePage.ViewModels;
using BntWeb.Utility.Extensions;
using BntWeb.Validation;
using BntWeb.Web.Extensions;
using static BntWeb.SinglePage.Permissions;

namespace BntWeb.SinglePage.Controllers
{
    public class AdminController : Controller
    {
        private readonly ICurrencyService _currencyService;
        private readonly IUserContainer _userContainer;
        private readonly ISinglePageService _singlePageService;
        // GET: Admin
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="currencyService"></param>
        /// <param name="userContainer"></param>
        /// <param name="singlePageService"></param>
        public AdminController(ICurrencyService currencyService, IUserContainer userContainer, ISinglePageService singlePageService)
        {
            _currencyService = currencyService;
            _userContainer = userContainer;
            _singlePageService = singlePageService;
        }
        [HttpGet]
        [AdminAuthorize(PermissionsArray = new[] { Permissions.EditPageKey })]
        public ActionResult ProEdit(string key)
        {
            Argument.ThrowIfNull(key, "key");
            Models.SinglePage singlepage = new Models.SinglePage();
            singlepage = _singlePageService.GetSinglePageByKey(key);
            Argument.ThrowIfNull(singlepage, "信息不存在");
            ViewBag.EditMode = true;
            return View("EditSinglePage", singlepage);
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.EditPageKey })]
        public ActionResult EditSinglePage(Guid id, bool isView)
        {
            var key = Request.Params["key"];
            Models.SinglePage singlepage;
            if (!string.IsNullOrWhiteSpace(key))
            {
                singlepage = _singlePageService.GetSinglePageByKey(key);
                ViewBag.IsView = isView;
                Argument.ThrowIfNull(singlepage, "信息不存在");
            }
            else
            {
                if (id == Guid.Empty)
                {
                    singlepage = new Models.SinglePage() { Id = Guid.Empty };
                }
                else
                {
                    singlepage = _singlePageService.GetSinglePageById(id);
                    Argument.ThrowIfNull(singlepage, "信息不存在");
                }
                ViewBag.EditMode = isView;
            }
            return View(singlepage);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult EditOnPost(EditSinglePageViewModel editSinglePage)
        {
            var result = new DataJsonResult();

            if (editSinglePage.Id == Guid.Empty)
            {
                var singlepage = new Models.SinglePage
                {
                    Id = KeyGenerator.GetGuidKey(),
                    Key = editSinglePage.Key,
                    Title = editSinglePage.Title,
                    Content = editSinglePage.Content,
                    SubTitle = editSinglePage.SubTitle,
                    LastUpdateTime = DateTime.Now,
                };
                Models.SinglePage singlePage = _singlePageService.GetSinglePageByKey(editSinglePage.Key);
                if (singlePage != null)
                {
                    result.ErrorMessage = "该Key已被使用，请重新填写";
                }
                else
                    _currencyService.Create(singlepage);
            }
            else
            {
                var tag = _currencyService.GetSingleById<Models.SinglePage>(editSinglePage.Id);
                Models.SinglePage singlePage = _singlePageService.GetSinglePageByKey(editSinglePage.Key);
                if (singlePage != null && singlePage.Id != editSinglePage.Id)
                {
                    result.ErrorMessage = "该Key已被使用，请重新填写";
                }
                else
                {
                    tag.Key = editSinglePage.Key;
                    tag.Title = editSinglePage.Title;
                    tag.Content = editSinglePage.Content;
                    tag.SubTitle = editSinglePage.SubTitle;
                    tag.LastUpdateTime = DateTime.Now;
                    _currencyService.Update(tag);
                }
            }
            return Json(result);
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ViewPageKey })]
        public ActionResult List()
        {
            return View();
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ViewPageKey })]
        public ActionResult ListOnPage()
        {
            var result = new DataTableJsonResult();

            //取参数值
            int draw, pageIndex, pageSize, totalCount;
            string sortColumn;
            bool isDesc;
            Request.GetDatatableParameters(out draw, out pageIndex, out pageSize, out sortColumn, out isDesc);
            result.draw = draw;

            //取查询条件
            var title = Request.Get("extra_search[Title]");
            var checkTitle = string.IsNullOrWhiteSpace(title);
            var lastUpdateTime = Request.Get("extra_search[LastUpdateTime]");
            var checklastUpdateTime = string.IsNullOrWhiteSpace(lastUpdateTime);
            var createlastUpdateTime = lastUpdateTime.To<DateTime>();


            Expression<Func<Models.SinglePage, bool>> expression =
                l =>
                    (checkTitle || l.Title.Contains(title)) &&
                    (checklastUpdateTime || l.LastUpdateTime >= createlastUpdateTime);
            Expression<Func<Models.SinglePage, object>> orderByExpression;
            //设置排序
            switch (sortColumn)
            {
                case "Title":
                    orderByExpression = act => new { act.Title };
                    break;
                case "LastUpdateTime":
                    orderByExpression = act => new { act.LastUpdateTime };
                    break;
                case "SubTitle":
                    orderByExpression = act => new { act.SubTitle };
                    break;
                case "Id":
                    orderByExpression = act => new { act.Id };
                    break;
                case "Content":
                    orderByExpression = act => new { act.Content };
                    break;
                case "key":
                    orderByExpression = act => new { act.Key };
                    break;
                default:
                    orderByExpression = act => new { act.Id };
                    break;
            }

            //分页查询
            var list = _singlePageService.GetListPaged(pageIndex, pageSize, expression, orderByExpression, isDesc, out totalCount);

            result.data = list;
            result.recordsTotal = totalCount;
            result.recordsFiltered = totalCount;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}