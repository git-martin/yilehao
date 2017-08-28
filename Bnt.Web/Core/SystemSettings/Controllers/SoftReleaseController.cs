using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using BntWeb.Caching;
using BntWeb.Core.SystemSettings.Models;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.Environment;
using BntWeb.FileSystems.Media;
using BntWeb.Logging;
using BntWeb.Mvc;
using BntWeb.Security;
using BntWeb.Security.Identity;
using BntWeb.Utility.Extensions;
using BntWeb.Web.Extensions;

namespace BntWeb.Core.SystemSettings.Controllers
{
    public class SoftReleaseController : Controller
    {
        private readonly IEnumerable<IBntWebModule> _modules;

        private readonly ICurrencyService _currencyService;

        public SoftReleaseController(IEnumerable<IBntWebModule> modules, ICurrencyService currencyService)
        {
            _modules = modules.OrderBy(m => m.InnerPosition);
            _currencyService = currencyService;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.SoftReleaseKey })]
        public ActionResult List()
        {
            ViewBag.Modules = _modules;
            return View();
        }


        [AdminAuthorize(PermissionsArray = new[] { Permissions.SoftReleaseKey })]
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
            var softType = Request.Get("extra_search[SoftType]");
            var checkSoftType = string.IsNullOrWhiteSpace(softType);
            var softTypeInt = softType.To<int>();

            var softName = Request.Get("extra_search[SoftName]");
            var checkSoftName = string.IsNullOrWhiteSpace(softName);

            var createTimeBegin = Request.Get("extra_search[CreateTimeBegin]");
            var checkCreateTimeBegin = string.IsNullOrWhiteSpace(createTimeBegin);
            var createTimeBeginTime = createTimeBegin.To<DateTime>();

            var createTimeEnd = Request.Get("extra_search[CreateTimeEnd]");
            var checkCreateTimeEnd = string.IsNullOrWhiteSpace(createTimeEnd);
            var createTimeEndTime = createTimeEnd.To<DateTime>();

            Expression<Func<SoftRelease, bool>> expression =
                l => (checkSoftType || (int)l.SoftType == softTypeInt) &&
                     (checkSoftName || l.SoftName.Contains(softName)) &&
                     (checkCreateTimeBegin || l.CreateTime >= createTimeBeginTime) &&
                     (checkCreateTimeEnd || l.CreateTime <= createTimeEndTime);

            result.data = _currencyService.GetListPaged(pageIndex, pageSize, expression, out totalCount, new OrderModelField { PropertyName = sortColumn, IsDesc = isDesc });
            result.recordsTotal = totalCount;
            result.recordsFiltered = totalCount;
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [AdminAuthorize(PermissionsArray = new[] { Permissions.SoftReleaseKey })]
        public ActionResult Edit(Guid? id)
        {
            var softRelease = new SoftRelease();
            if (id != null)
                softRelease = _currencyService.GetSingleById<SoftRelease>(id);

            return View(softRelease);
        }
        

        [AdminAuthorize(PermissionsArray = new[] { Permissions.SoftReleaseKey })]
        public ActionResult EditOnPost([FromBody]SoftRelease softRelease)
        {
            var result = new DataJsonResult();

            try
            {
                SoftRelease oldSoftRelease = null;
                if (softRelease.Id != Guid.Empty)
                    oldSoftRelease = _currencyService.GetSingleById<SoftRelease>(softRelease.Id);

                var downloadUrl = softRelease.DownloadUrl;
                if (oldSoftRelease == null)
                {
                    softRelease.SoftKey = softRelease.SoftType.ToString();
                    softRelease.Id = KeyGenerator.GetGuidKey();
                    softRelease.CreateTime = DateTime.Now;
                    
                    _currencyService.Create(softRelease);
                }
                else
                {
                    oldSoftRelease.SoftType = softRelease.SoftType;
                    oldSoftRelease.SoftKey = softRelease.SoftType.ToString();
                    oldSoftRelease.Description = softRelease.Description;
                    oldSoftRelease.DownloadUrl = softRelease.DownloadUrl;
                    oldSoftRelease.Version = softRelease.Version;
                    oldSoftRelease.SoftName = softRelease.SoftName;
                    oldSoftRelease.ForceUpdating = softRelease.ForceUpdating;
                    _currencyService.Update(oldSoftRelease);
                }

            }
            catch (Exception ex)
            {
                result.ErrorMessage = "保存出现意外错误";
                Logger.Error(ex, "保存出错了");
            }

            return Json(result);
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.SoftReleaseKey })]
        public ActionResult Delete(Guid id)
        {

            var result = new DataJsonResult();

            if (_currencyService.DeleteByConditon<SoftRelease>(sr => sr.Id.Equals(id)) <= 0)
            {
                result.ErrorMessage = "删除发生错误，删除失败";
            }

            return Json(result);
        }
    }
}