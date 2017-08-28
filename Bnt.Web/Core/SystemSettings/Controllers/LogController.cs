using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using BntWeb.Caching;
using BntWeb.Core.SystemSettings.Models;
using BntWeb.Data.Services;
using BntWeb.Environment;
using BntWeb.Logging;
using BntWeb.Mvc;
using BntWeb.Security;
using BntWeb.Security.Identity;
using BntWeb.Utility.Extensions;
using BntWeb.Web.Extensions;

namespace BntWeb.Core.SystemSettings.Controllers
{
    public class LogController : Controller
    {
        private readonly IEnumerable<IBntWebModule> _modules;

        private readonly ICurrencyService _currencyService;

        public LogController(IEnumerable<IBntWebModule> modules, ICurrencyService currencyService)
        {
            _modules = modules.OrderBy(m => m.InnerPosition);
            _currencyService = currencyService;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ViewLogKey })]
        public ActionResult List()
        {
            ViewBag.Modules = _modules;
            return View();
        }


        [AdminAuthorize(PermissionsArray = new[] { Permissions.ViewLogKey })]
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
            var moduleKey = Request.Get("extra_search[ModuleKey]");
            var checkModuleKey = string.IsNullOrWhiteSpace(moduleKey);

            var securityLevel = Request.Get("extra_search[SecurityLevel]");
            var checkSecurityLevel = string.IsNullOrWhiteSpace(securityLevel);
            var securityLevelInt = securityLevel.To<int>();

            var userName = Request.Get("extra_search[UserName]");
            var checkUserName = string.IsNullOrWhiteSpace(userName);

            var createTimeBegin = Request.Get("extra_search[CreateTimeBegin]");
            var checkCreateTimeBegin = string.IsNullOrWhiteSpace(createTimeBegin);
            var createTimeBeginTime = createTimeBegin.To<DateTime>();

            var createTimeEnd = Request.Get("extra_search[CreateTimeEnd]");
            var checkCreateTimeEnd = string.IsNullOrWhiteSpace(createTimeEnd);
            var createTimeEndTime = createTimeEnd.To<DateTime>();

            Expression<Func<SystemLog, bool>> expression =
                l => (checkModuleKey || l.ModuleKey.Equals(moduleKey, StringComparison.OrdinalIgnoreCase)) &&
                     (checkSecurityLevel || (int)l.SecurityLevel == securityLevelInt) &&
                     (checkUserName || l.UserName.Contains(userName)) &&
                     (checkCreateTimeBegin || l.CreateTime >= createTimeBeginTime) &&
                     (checkCreateTimeEnd || l.CreateTime <= createTimeEndTime);

            result.data = _currencyService.GetListPaged(pageIndex, pageSize, expression, out totalCount, new OrderModelField { PropertyName = sortColumn, IsDesc = isDesc });
            result.recordsTotal = totalCount;
            result.recordsFiltered = totalCount;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}