using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using BntWeb.Data.Services;
using BntWeb.Feedback.Models;
using BntWeb.Feedback.Services;
using BntWeb.Feedback.ViewModels;
using BntWeb.FileSystems.Media;
using BntWeb.Mvc;
using BntWeb.Security;
using BntWeb.Utility.Extensions;
using BntWeb.Validation;
using BntWeb.Web.Extensions;

namespace BntWeb.Feedback.Controllers
{
    public class AdminController : Controller
    {
        private readonly IFeedbackService _feedbackService;
        private readonly IStorageFileService _storageFileService;
        private readonly ICurrencyService _currencyService;
        private readonly IUserContainer _userContainer;

        /// <summary>
        /// 定义文件类型
        /// </summary>
        private const string FileTypeName = "FeedbackImage";

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="feedbackService"></param>
        /// <param name="storageFileService"></param>
        /// <param name="currencyService"></param>
        /// <param name="userContainer"></param>
        public AdminController(IFeedbackService feedbackService, IStorageFileService storageFileService, ICurrencyService currencyService, IUserContainer userContainer)
        {
            _feedbackService = feedbackService;
            _storageFileService = storageFileService;
            _currencyService = currencyService;
            _userContainer = userContainer;
        }

        [AdminAuthorize]
        public ActionResult List(string sourceId, string feedbackType = "1", string sourceType = "Feedbacks")
        {
            //var feedbackType = Request.Get("feedbackType","1");
            //var sourceType = Request.Get("sourceType", "Feedbacks");
            //var sourceId = Request.Get("sourceId");
            ViewBag.FeedbackType = feedbackType;
            ViewBag.SourceType = sourceType;
            ViewBag.SourceId = sourceId;

            return View();
        }

        [AdminAuthorize]
        public ActionResult ListOnPage()
        {
            //return Json("", JsonRequestBehavior.AllowGet);
            var result = new DataTableJsonResult();

            //取参数值
            int draw, pageIndex, pageSize, totalCount;
            string sortColumn;
            bool isDesc;
            Request.GetDatatableParameters(out draw, out pageIndex, out pageSize, out sortColumn, out isDesc);
            result.draw = draw;

            //取查询条件
            //var type = Request.Get("extra_search[FeedbackType]");
            //var checkTypeId = string.IsNullOrWhiteSpace(type);

            var feedbackType = Request.Get("extra_search[FeedbackType]", "1").To<int>();

            var sourceType = Request.Get("extra_search[SourceType]");
            var checkSourceType = string.IsNullOrWhiteSpace(sourceType);

            var sourceId = Request.Get("extra_search[SourceId]");
            var checkSourceId = string.IsNullOrWhiteSpace(sourceId);

            var status = Request.Get("extra_search[ProcesseStatus]");
            var checkStatus = string.IsNullOrWhiteSpace(status);
            var statusInt = status.To<int>();

            var createTimeBegin = Request.Get("extra_search[CreateTimeBegin]");
            var checkCreateTimeBegin = string.IsNullOrWhiteSpace(createTimeBegin);
            var createTimeBeginTime = createTimeBegin.To<DateTime>();

            var createTimeEnd = Request.Get("extra_search[CreateTimeEnd]");
            var checkCreateTimeEnd = string.IsNullOrWhiteSpace(createTimeEnd);
            var createTimeEndTime = createTimeEnd.To<DateTime>();

            Expression<Func<Models.Feedback, bool>> expression =
                l => (checkSourceType || l.SourceType.Equals(sourceType, StringComparison.OrdinalIgnoreCase)) &&
                     (checkSourceId || l.SourceId.ToString().Equals(sourceId)) &&
                     (int)l.FeedbackType == feedbackType &&
                     (checkCreateTimeBegin || l.CreateTime >= createTimeBeginTime) &&
                     (checkCreateTimeEnd || l.CreateTime <= createTimeEndTime) &&
                     (checkStatus || (int)l.ProcesseStatus == statusInt) &&
                     ((int)l.Status > (int)FeedbackStatus.Delete);


            Expression<Func<Models.Feedback, object>> orderByExpression;
            //设置排序
            switch (sortColumn)
            {
                default:
                    orderByExpression = act => new { act.CreateTime };
                    break;
            }

            //分页查询
            var list = _feedbackService.GetListPaged(pageIndex, pageSize, expression, orderByExpression,
                isDesc, out totalCount);

            result.data = list;
            result.recordsTotal = totalCount;
            result.recordsFiltered = totalCount;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AdminAuthorize]
        public ActionResult Processe(Guid id, bool isView)
        {
            bool editMode = !isView;
            ViewBag.EditMode = editMode;
            ViewBag.IsView = isView;


            Models.Feedback feedback = _feedbackService.GetFeedbackById(id);
            Argument.ThrowIfNull(feedback, "反馈信息不存在");

            return View(feedback);
        }

        /// <summary>
        /// 反馈处理
        /// </summary>
        /// <param name="processeFeedback"></param>
        /// <returns></returns>
        [HttpPost]
        [AdminAuthorize]
        public ActionResult ProcesseOnPost(ProcesseFeedbackViewModel processeFeedback)
        {
            var result = new DataJsonResult();
            Models.Feedback model = new Models.Feedback();

            if (!string.IsNullOrWhiteSpace(processeFeedback.Id.ToString()) && processeFeedback.Id != Guid.Empty)
                model = _feedbackService.GetFeedbackById(processeFeedback.Id);

            if (model == null)
            {
                result.ErrorMessage = "请选择要处理的反馈信息";
            }
            else
            {
                model.ProcesseStatus = processeFeedback.ProcesseStatus;
                model.ProcesseRemark = processeFeedback.ProcesseRemark;
                var currentUser = _userContainer.CurrentUser;
                model.ProcesseUserId = currentUser.Id.ToGuid();
                model.ProcesseTime = DateTime.Now;

                bool res = _feedbackService.UpdateFeedback(model); ;
                if (!res)
                {
                    result.ErrorMessage = "处理失败";
                }
            }

            return Json(result);
        }

        /// <summary>
        /// 只改变状态  列表上直接处理时可使用
        /// </summary>
        /// <param name="feedbackId"></param>
        /// <returns></returns>
        [HttpPost]
        [AdminAuthorize]
        public ActionResult SaveProcesse(Guid feedbackId)
        {
            var result = new DataJsonResult();
            Models.Feedback feedback = _currencyService.GetSingleById<Models.Feedback>(feedbackId);

            if (feedback != null)
            {
                feedback.ProcesseStatus = ProcesseStatus.Processed;
                var currentUser = _userContainer.CurrentUser;
                feedback.ProcesseUserId = currentUser.Id.ToGuid();
                feedback.ProcesseTime = DateTime.Now;
                bool res = _feedbackService.UpdateFeedback(feedback);
                if (!res)
                {
                    result.ErrorMessage = "处理失败";
                }
            }
            else
            {
                result.ErrorMessage = "反馈不存在！";
            }

            return Json(result);
        }



        [HttpPost]
        [AdminAuthorize]
        public ActionResult Delete(Guid feedbackId)
        {
            var result = new DataJsonResult();
            Models.Feedback feedback = _currencyService.GetSingleById<Models.Feedback>(feedbackId);

            if (feedback != null)
            {
                var isDelete = _feedbackService.Delete(feedback);

                if (!isDelete)
                {
                    result.ErrorMessage = "删除失败!";
                }
            }
            else
            {
                result.ErrorMessage = "反馈不存在！";
            }

            return Json(result);
        }

    }
}