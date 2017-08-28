using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using BntWeb.Core.SystemSettings.Models;
using BntWeb.Core.SystemSettings.Services;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.Mvc;
using BntWeb.Security;
using BntWeb.Utility.Extensions;
using BntWeb.Web.Extensions;

namespace BntWeb.Core.SystemSettings.Controllers
{
    public class DbBackupController : Controller
    {
        private readonly IBackupService _backupService;
        private readonly ICurrencyService _currencyService;
        private readonly IUserContainer _userContainer;

        public DbBackupController(IBackupService backupService, ICurrencyService currencyService, IUserContainer userContainer)
        {
            _backupService = backupService;
            _currencyService = currencyService;
            _userContainer = userContainer;
        }


        [AdminAuthorize(PermissionsArray = new[] { Permissions.DbBackupKey })]
        public ActionResult List()
        {
            return View();
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.DbBackupKey })]
        public ActionResult ListOnPage()
        {
            var result = new DataTableJsonResult();

            //取参数值
            int draw, pageIndex, pageSize, totalCount;
            string sortColumn;
            bool isDesc;
            Request.GetDatatableParameters(out draw, out pageIndex, out pageSize, out sortColumn, out isDesc);
            result.draw = draw;

            var userName = Request.Get("extra_search[UserName]");
            var checkUserName = string.IsNullOrWhiteSpace(userName);

            var createTimeBegin = Request.Get("extra_search[CreateTimeBegin]");
            var checkCreateTimeBegin = string.IsNullOrWhiteSpace(createTimeBegin);
            var createTimeBeginTime = createTimeBegin.To<DateTime>();

            var createTimeEnd = Request.Get("extra_search[CreateTimeEnd]");
            var checkCreateTimeEnd = string.IsNullOrWhiteSpace(createTimeEnd);
            var createTimeEndTime = createTimeEnd.To<DateTime>().DayEnd();

            Expression<Func<BackupInfo, bool>> expression =
                l => (checkUserName || l.CreateUserName.Contains(userName)) &&
                     (checkCreateTimeBegin || l.CreateTime >= createTimeBeginTime) &&
                     (checkCreateTimeEnd || l.CreateTime <= createTimeEndTime);

            result.data = _currencyService.GetListPaged(pageIndex, pageSize, expression, out totalCount, new OrderModelField { PropertyName = sortColumn, IsDesc = isDesc });
            result.recordsTotal = totalCount;
            result.recordsFiltered = totalCount;
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [AdminAuthorize(PermissionsArray = new[] { Permissions.DbBackupKey })]
        public ActionResult Backup()
        {
            var result = new DataJsonResult();
            if (_backupService.IsBusy)
            {
                result.ErrorMessage = "正在备份，请稍后再操作";
            }
            else
            {
                var backupInfo = new BackupInfo
                {
                    Id = KeyGenerator.GetGuidKey(),
                    CreateUserId = _userContainer.CurrentUser.Id,
                    CreateUserName = _userContainer.CurrentUser.UserName,
                    Status = BackupStatus.Doing,
                    CreateTime = DateTime.Now
                };

                _currencyService.Create(backupInfo);
                _backupService.Backup(backupInfo);
            }

            return Json(result);
        }
    }
}