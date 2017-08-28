using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using BntWeb.Data.Services;
using BntWeb.Logistics.Models;
using BntWeb.Mvc;
using BntWeb.OrderProcess.Models;
using BntWeb.OrderProcess.Services;
using BntWeb.OrderProcess.ViewModels;
using BntWeb.Security;
using BntWeb.Utility.Extensions;
using BntWeb.Web.Extensions;

namespace BntWeb.OrderProcess.Controllers
{
    public class RefundController : Controller
    {
        private readonly ICurrencyService _currencyService;
        private readonly IOrderService _orderService;
        private readonly IRefundService _refundService;

        public RefundController(ICurrencyService currencyService, IOrderService orderService, IRefundService refundService)
        {
            _currencyService = currencyService;
            _orderService = orderService;
            _refundService = refundService;
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ViewOrderKey })]
        public ActionResult List()
        {
            return View();
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ViewOrderKey })]
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
            var orderNo = Request.Get("extra_search[OrderNo]");
            var checkOrderNo = string.IsNullOrWhiteSpace(orderNo);

            var consignee = Request.Get("extra_search[Consignee]");
            var checkConsignee = string.IsNullOrWhiteSpace(consignee);

            var createTimeBegin = Request.Get("extra_search[CreateTimeBegin]");
            var checkCreateTimeBegin = string.IsNullOrWhiteSpace(createTimeBegin);
            var createTimeBeginTime = createTimeBegin.To<DateTime>();

            var createTimeEnd = Request.Get("extra_search[CreateTimeEnd]");
            var checkCreateTimeEnd = string.IsNullOrWhiteSpace(createTimeEnd);
            var createTimeEndTime = createTimeEnd.To<DateTime>();

            Expression<Func<Order, bool>> expression =
                l => (checkOrderNo || l.OrderNo.Contains(orderNo)) &&
                     (checkConsignee || l.Consignee.Contains(consignee)) &&
                     l.OrderStatus != OrderStatus.Deleted &&
                     l.RefundStatus == OrderRefundStatus.Refunding &&
                     (checkCreateTimeBegin || l.CreateTime >= createTimeBeginTime) &&
                     (checkCreateTimeEnd || l.CreateTime <= createTimeEndTime);


            //分页查询
            var list = _currencyService.GetListPaged<Order>(pageIndex, pageSize, expression, out totalCount, new OrderModelField { PropertyName = sortColumn, IsDesc = isDesc });

            result.data = list;
            result.recordsTotal = totalCount;
            result.recordsFiltered = totalCount;
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [AdminAuthorize(PermissionsArray = new[] { Permissions.ViewOrderKey })]
        public ActionResult Detail(Guid orderId)
        {
            var order = _orderService.Load(orderId);
            var refundList=_currencyService.GetList<OrderRefund>(s => s.OrderId == orderId).OrderBy(x=>x.CreateTime).Select(o => new RefundViewModel(o)).ToList();
            ViewBag.RefundList = refundList;
            return View(order);
        }

        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="applyId"></param>
        /// <param name="auditResult"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageOrderKey })]
        public ActionResult Audit(Guid applyId, int auditResult, string remark)
        {
            var result = new DataJsonResult();
            var refund = _currencyService.GetSingleById<OrderRefund>(applyId);
            if (refund.RefundStatus != RefundStatus.Applying)
            {
                result.ErrorMessage = "退款状态不合法";
            }
            else
            {
                refund.ReviewResult = auditResult == 1 ? ReviewResult.Passed : ReviewResult.UnPassed;
                refund.ReviemMemo = remark;
                _refundService.AuditRefund(refund);
            }
            return Json(result);
        }

        /// <summary>
        /// 退款打款
        /// </summary>
        /// <param name="applyId"></param>
        /// <returns></returns>
        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageOrderKey })]
        public ActionResult Pay(Guid applyId)
        {
            var result = new DataJsonResult();
            var refund = _currencyService.GetSingleById<OrderRefund>(applyId);
            if (refund.RefundStatus == RefundStatus.Processed && refund.ReviewResult == ReviewResult.Passed)
            {
                _refundService.PayRefund(refund);
            }
            else
            {
                result.ErrorMessage = "操作不合法";//只有退款类型为退款仅退货且审核通过的退款申请才有打款操作
            }
            return Json(result);
        }
    }
}