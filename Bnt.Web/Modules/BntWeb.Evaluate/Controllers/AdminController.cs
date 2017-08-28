using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BntWeb.Evaluate.Services;
using BntWeb.Mvc;
using BntWeb.Security;
using BntWeb.Utility.Extensions;
using BntWeb.Web.Extensions;

namespace BntWeb.Evaluate.Controllers
{
    public class AdminController : Controller
    {
        private readonly IEvaluateService _evaluateService;
        public AdminController(IEvaluateService evaluateService)
        {
            _evaluateService = evaluateService;
        }

        [AdminAuthorize]
        public ActionResult List(string sourceType, Guid sourceId)
        {
            ViewBag.SourceType = sourceType;
            ViewBag.SourceId = sourceId;
            return View();
        }

        [AdminAuthorize]
        public ActionResult ListOnPage(string sourceType, Guid sourceId)
        {
            var result = new DataJsonResult();
            var pageIndex = Request.Get("pageIndex").To(1);
            var pageSize = Request.Get("pageSize").To(10);
            int totalCount;
            var evaluates = _evaluateService.LoadByPage(sourceId, null, sourceType, out totalCount, pageIndex, pageSize);
            result.Data = new { TotalCount = totalCount, Evaluates = evaluates };
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [AdminAuthorize]
        public ActionResult Delete(string evaluateId)
        {
            var result = new DataJsonResult();

            _evaluateService.DeleteEvaluate(evaluateId.ToGuid());

            return Json(result);
        }

        [AdminAuthorize]
        public ActionResult BatchApproveEvaluates(List<Guid> evaluatesIds)
        {
            var result = new DataJsonResult();
            if (!_evaluateService.ApprvoeEvaluate(evaluatesIds))
            {
                result.ErrorMessage = "未知异常，审核失败";
            }

            return Json(result);
        }
        [AdminAuthorize]
        public ActionResult BatchDeleteEvaluates(List<Guid> evaluatesIds)
        {
            var result = new DataJsonResult();
            if (!_evaluateService.DeleteEvaluates(evaluatesIds))
            {
                result.ErrorMessage = "位置异常，删除失败";

            }
            return Json(result);
        }
    }
}