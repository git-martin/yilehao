using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using BntWeb.Wallet.Services;
using BntWeb.Mvc;
using BntWeb.Security;
using BntWeb.SystemMessage.Models;
using BntWeb.SystemMessage.Services;
using BntWeb.Utility.Extensions;
using BntWeb.Web.Extensions;
using BntWeb.WebApi.Models;

namespace BntWeb.Wallet.Controllers
{
    public class AdminController : Controller
    {
        private readonly IWalletService _walletService;
        private readonly ISystemMessageService _systemMessageService;

        public AdminController(IWalletService walletService, ISystemMessageService systemMessageService)
        {
            _walletService = walletService;
            _systemMessageService = systemMessageService;
        }

        [AdminAuthorize]
        public ActionResult List()
        {
            return View();
        }

        [AdminAuthorize]
        public ActionResult CrashApplyList()
        {
            return View();
        }

        [AdminAuthorize]
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
            var applyStateType = Request.Get("extra_search[ApplyStateType]");
            var checkApplyStateType = string.IsNullOrWhiteSpace(applyStateType);
            var applyStateTypeInt = applyStateType.To<int>();

            var paymentType = Request.Get("extra_search[PaymentType]");
            var checkPaymentType = string.IsNullOrWhiteSpace(paymentType);
            var paymentTypeInt = paymentType.To<int>();

            Expression<Func<Models.CrashApply, bool>> expression = null;

            expression = c => (checkApplyStateType || (int)c.ApplyState == applyStateTypeInt) && (checkPaymentType || (int)c.PaymentType == paymentTypeInt);
            Expression<Func<Models.CrashApply, object>> orderByExpression;

            switch (sortColumn)
            {
                case "Money":
                    orderByExpression = c => new { c.Money };
                    break;
                case "CreateTime":
                    orderByExpression = c => new { c.CreateTime };
                    break;
                default:
                    orderByExpression = c => new { c.CreateTime };
                    break;
            }

            //分页查询
            var list = _walletService.GetCrashApplyByPage(pageIndex, pageSize, expression, orderByExpression, isDesc, out totalCount);

            result.data = list;
            result.recordsTotal = totalCount;
            result.recordsFiltered = totalCount;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 确认打款
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AdminAuthorize]
        public ActionResult AuditApply()
        {
            string id = Request.Params["id"];
            var result = new DataJsonResult();
            if (!_walletService.AuditApply(id.ToGuid()))
            {
                result.ErrorMessage = "未知异常";
            }
            return Json(result);
        }
    }
}