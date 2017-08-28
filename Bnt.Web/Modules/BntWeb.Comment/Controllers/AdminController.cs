using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using BntWeb.Comment.Services;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.Mvc;
using BntWeb.Security;
using BntWeb.Utility.Extensions;
using BntWeb.Web.Extensions;

namespace BntWeb.Comment.Controllers
{
    public class AdminController : Controller
    {
        private readonly ICommentService _commentService;
        public AdminController(ICommentService commentService)
        {
            _commentService = commentService;
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
            var comments = _commentService.LoadByPage(sourceId, null, sourceType, out totalCount, pageIndex, pageSize);
            result.Data = new { TotalCount = totalCount, Comments = comments };
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [AdminAuthorize]
        public ActionResult Delete(string commentId)
        {
            var result = new DataJsonResult();

            _commentService.DeleteComment(commentId.ToGuid());

            return Json(result);
        }
    }
}