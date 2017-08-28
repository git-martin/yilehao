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
    public class SinglePageController : Controller
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
        public SinglePageController(ICurrencyService currencyService, IUserContainer userContainer, ISinglePageService singlePageService)
        {
            _currencyService = currencyService;
            _userContainer = userContainer;
            _singlePageService = singlePageService;
        }

        public ActionResult Page(string key)
        {
            Argument.ThrowIfNull(key, "key");
            var singlepage = _singlePageService.GetSinglePageByKey(key);
            Argument.ThrowIfNull(singlepage, "信息不存在");
            ViewBag.EditMode = true;
            return View(singlepage);
        }
    }
}