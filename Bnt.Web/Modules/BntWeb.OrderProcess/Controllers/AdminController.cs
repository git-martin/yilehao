using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.Logging;
using BntWeb.Logistics.Models;
using BntWeb.OrderProcess.Models;
using BntWeb.Mvc;
using BntWeb.OrderProcess.Services;
using BntWeb.Security;
using BntWeb.Services;
using BntWeb.Utility.Extensions;
using BntWeb.Wallet.Services;
using BntWeb.Web.Extensions;
using System.IO;
using NPOI.HSSF.UserModel;

namespace BntWeb.OrderProcess.Controllers
{
    public class AdminController : Controller
    {
        private readonly ICurrencyService _currencyService;
        private readonly IOrderService _orderService;
        private readonly IUserContainer _userContainer;

        public AdminController(ICurrencyService currencyService, IOrderService orderService, IUserContainer userContainer)
        {
            _currencyService = currencyService;
            _orderService = orderService;
            _userContainer = userContainer;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ViewOrderKey })]
        public ActionResult List()
        {
            ViewBag.OrderStatus = Request.Get("OrderStatus");
            ViewBag.RefundStatus = Request.Get("RefundStatus");
            ViewBag.MemberName = Request.Get("MemberName");
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

            var memberName = Request.Get("extra_search[MemberName]");
            var checkMemberName = string.IsNullOrWhiteSpace(memberName);

            var orderStatus = Request.Get("extra_search[OrderStatus]");
            var checkOrderStatus = string.IsNullOrWhiteSpace(orderStatus);
            var orderStatusInt = orderStatus.To<int>();

            var refundStatus = Request.Get("extra_search[RefundStatus]");
            var checkRefundStatus = string.IsNullOrWhiteSpace(refundStatus);
            var refundStatusInt = refundStatus.To<int>();

            var payStatus = Request.Get("extra_search[PayStatus]");
            var checkPayStatus = string.IsNullOrWhiteSpace(payStatus);
            var payStatusInt = payStatus.To<int>();

            var shippingStatus = Request.Get("extra_search[ShippingStatus]");
            var checkShippingStatus = string.IsNullOrWhiteSpace(shippingStatus);
            var shippingStatusInt = shippingStatus.To<int>();

            var paymentId = Request.Get("extra_search[PaymentId]");
            var checkPaymentId = string.IsNullOrWhiteSpace(paymentId);

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
                     (checkOrderStatus || (int)l.OrderStatus == orderStatusInt) &&
                     (checkRefundStatus || (int)l.RefundStatus == refundStatusInt) &&
                     (checkPayStatus || (int)l.PayStatus == payStatusInt) &&
                     (checkShippingStatus || (int)l.ShippingStatus == shippingStatusInt) &&
                     (checkCreateTimeBegin || l.CreateTime >= createTimeBeginTime) &&
                     (checkCreateTimeEnd || l.CreateTime <= createTimeEndTime) &&
                     (checkPaymentId || l.PaymentId.ToString() == paymentId) &&
                     (checkMemberName || l.MemberName.Contains(memberName));


            //分页查询
            var list = _currencyService.GetListPaged<Order>(pageIndex, pageSize, expression, out totalCount, new[] { new OrderModelField { PropertyName = sortColumn, IsDesc = isDesc } }, new[] { "OrderGoods" }).Select(o => new ViewModels.SimpleOderModel(o));

            result.data = list;
            result.recordsTotal = totalCount;
            result.recordsFiltered = totalCount;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ViewOrderKey })]
        public ActionResult Detail(Guid orderId)
        {
            var order = _orderService.Load(orderId);

            ViewBag.Shippings = _currencyService.GetList<Shipping>(s => s.Status == Logistics.Models.ShippingStatus.Enabled).ToJson();

            return View(order);
        }

        /// <summary>
        /// 关闭订单
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="memo"></param>
        /// <returns></returns>
        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageOrderKey })]
        public ActionResult Close(Guid orderId, string memo)
        {
            var result = new DataJsonResult();
            if (!_orderService.ChangeOrderStatus(orderId, OrderStatus.Closed, null, null, null, memo))
            {
                result.ErrorCode = "关闭订单出现异常错误";
            }
            return Json(result);
        }

        /// <summary>
        /// 发货
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="shippingId"></param>
        /// <param name="shippingName"></param>
        /// <param name="shippingCode"></param>
        /// <param name="shippingNo"></param>
        /// <returns></returns>
        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageOrderKey })]
        public ActionResult Delivery(Guid orderId, Guid shippingId, string shippingName, string shippingCode, string shippingNo)
        {
            var result = new DataJsonResult();
            var order = _orderService.Load(orderId);
            if (order.OrderStatus != OrderStatus.WaitingForDelivery)
            {
                result.ErrorMessage = "订单状态不合法";
            }
            else
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    if (_orderService.SetShippingInfo(orderId, shippingId, shippingName, shippingCode, shippingNo))
                        if (_orderService.ChangeOrderStatus(orderId, OrderStatus.WaitingForReceiving, null, Models.ShippingStatus.Shipped, null, "订单发货"))
                            //提交
                            scope.Complete();
                }

                //删除订单发货提醒记录
                _currencyService.DeleteByConditon<OrderDeliveryReminder>(x => x.OrderId == orderId);
            }

            return Json(result);
        }

        /// <summary>
        /// 发货
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="shippingId"></param>
        /// <param name="shippingName"></param>
        /// <param name="shippingCode"></param>
        /// <param name="shippingNo"></param>
        /// <returns></returns>
        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageOrderKey })]
        public ActionResult ChangeShipping(Guid orderId, Guid shippingId, string shippingName, string shippingCode, string shippingNo)
        {
            var result = new DataJsonResult();
            var order = _orderService.Load(orderId);
            if (order.OrderStatus != OrderStatus.WaitingForReceiving)
            {
                result.ErrorMessage = "订单状态不合法";
            }
            else
            {
                if (!_orderService.SetShippingInfo(orderId, shippingId, shippingName, shippingCode, shippingNo))
                {
                    result.ErrorMessage = "物流信息更新失败";
                }
            }

            return Json(result);
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageOrderKey })]
        public ActionResult ChangePrice(Guid orderId, Guid orderGoodsId, decimal goodsPrice)
        {
            var result = new DataJsonResult();
            try
            {
                if (!_orderService.ChangePrice(orderId, orderGoodsId, goodsPrice))
                {
                    result.ErrorMessage = "异常错误，修改失败";
                }
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }


            return Json(result);
        }


        public FileResult OrderToExecl()
        {
            //取查询条件
            var orderNo = Request.Get("OrderNo");
            var checkOrderNo = string.IsNullOrWhiteSpace(orderNo);

            var consignee = Request.Get("Consignee");
            var checkConsignee = string.IsNullOrWhiteSpace(consignee);

            var memberName = Request.Get("MemberName");
            var checkMemberName = string.IsNullOrWhiteSpace(memberName);

            var orderStatus = Request.Get("OrderStatus");
            var checkOrderStatus = string.IsNullOrWhiteSpace(orderStatus);
            var orderStatusInt = orderStatus.To<int>();

            var refundStatus = Request.Get("RefundStatus");
            var checkRefundStatus = string.IsNullOrWhiteSpace(refundStatus);
            var refundStatusInt = refundStatus.To<int>();

            var payStatus = Request.Get("PayStatus");
            var checkPayStatus = string.IsNullOrWhiteSpace(payStatus);
            var payStatusInt = payStatus.To<int>();

            var shippingStatus = Request.Get("ShippingStatus");
            var checkShippingStatus = string.IsNullOrWhiteSpace(shippingStatus);
            var shippingStatusInt = shippingStatus.To<int>();

            var paymentId = Request.Get("PaymentId");
            var checkPaymentId = string.IsNullOrWhiteSpace(paymentId);

            var createTimeBegin = Request.Get("CreateTimeBegin");
            var checkCreateTimeBegin = string.IsNullOrWhiteSpace(createTimeBegin);
            var createTimeBeginTime = createTimeBegin.To<DateTime>();

            var createTimeEnd = Request.Get("CreateTimeEnd");
            var checkCreateTimeEnd = string.IsNullOrWhiteSpace(createTimeEnd);
            var createTimeEndTime = createTimeEnd.To<DateTime>();

            Expression<Func<Order, bool>> expression =
                l => (checkOrderNo || l.OrderNo.Contains(orderNo)) &&
                     (checkConsignee || l.Consignee.Contains(consignee)) &&
                     l.OrderStatus != OrderStatus.Deleted &&
                     (checkOrderStatus || (int)l.OrderStatus == orderStatusInt) &&
                     (checkRefundStatus || (int)l.RefundStatus == refundStatusInt) &&
                     (checkPayStatus || (int)l.PayStatus == payStatusInt) &&
                     (checkShippingStatus || (int)l.ShippingStatus == shippingStatusInt) &&
                     (checkCreateTimeBegin || l.CreateTime >= createTimeBeginTime) &&
                     (checkCreateTimeEnd || l.CreateTime <= createTimeEndTime) &&
                     (checkPaymentId || l.PaymentId.ToString() == paymentId) &&
                     (checkMemberName || l.MemberName.Contains(memberName));

            var list = _orderService.GetList(expression).Select(o => new ViewModels.SimpleOderModel(o));

            //创建Excel文件的对象  
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();

            //添加一个sheet  
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");
            if (list.Any())
            {
                //给sheet1添加第一行的头部标题  
                NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
                row1.CreateCell(0).SetCellValue("订单号");
                row1.CreateCell(1).SetCellValue("商品名称");
                row1.CreateCell(2).SetCellValue("规格");
                row1.CreateCell(3).SetCellValue("数量");
                row1.CreateCell(4).SetCellValue("下单时间");
                row1.CreateCell(5).SetCellValue("收货人");
                row1.CreateCell(6).SetCellValue("收货地址");
                row1.CreateCell(7).SetCellValue("联系电话");
                row1.CreateCell(8).SetCellValue("商品总价");
                row1.CreateCell(9).SetCellValue("物流费用");
                row1.CreateCell(10).SetCellValue("积分折抵");
                row1.CreateCell(11).SetCellValue("应付金额");
                row1.CreateCell(12).SetCellValue("订单状态");
                var i = 0;
                var cs = book.CreateCellStyle();
                cs.WrapText = true;
                foreach (var item in list)
                {
                    string n = "";
                    string m = "";
                    string k = "";
                    for (int j = 0; j < item.OrderGoods.Count; j++)
                    {
                        if (j == item.OrderGoods.Count - 1)
                        {
                            n += item.OrderGoods[j].GoodsName;
                            m += item.OrderGoods[j].GoodsAttribute;
                            k += item.OrderGoods[j].Quantity + item.OrderGoods[j].Unit;
                        }
                        else
                        {
                            n += item.OrderGoods[j].GoodsName + "\n";
                            m += item.OrderGoods[j].GoodsAttribute+"\n";
                            k += item.OrderGoods[j].Quantity + item.OrderGoods[j].Unit+"\n";
                        }

                    }
                    NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(item.OrderNo);
                    rowtemp.CreateCell(1).SetCellValue(n);
                    rowtemp.CreateCell(2).SetCellValue(m);
                    rowtemp.CreateCell(3).SetCellValue(k);
                    if (item.OrderGoods.Count > 1)
                    {
                        rowtemp.GetCell(1).CellStyle = cs;
                        rowtemp.GetCell(2).CellStyle = cs;
                        rowtemp.GetCell(3).CellStyle = cs;
                    }
                    rowtemp.CreateCell(4).SetCellValue(string.Format("{0:yyyy-MM-dd HH:mm:ss}", item.CreateTime));
                    rowtemp.CreateCell(5).SetCellValue(item.Consignee);
                    rowtemp.CreateCell(6).SetCellValue(item.RegionName+item.Address);
                    rowtemp.CreateCell(7).SetCellValue(item.Tel);
                    rowtemp.CreateCell(8).SetCellValue(item.GoodsAmount.ToString("#0.00"));
                    rowtemp.CreateCell(9).SetCellValue(item.ShippingFee.ToString("#0.00"));
                    rowtemp.CreateCell(10).SetCellValue(item.IntegralMoney.ToString("#0.00"));
                    rowtemp.CreateCell(11).SetCellValue(item.PayFee.ToString("#0.00"));
                    var statusName = item.OrderStatus.Description();
                    if (item.RefundStatus > 0)
                        statusName += "(" + item.RefundStatus.Description() + ")";
                    if (item.EvaluateStatus > 0)
                        statusName += "(" + item.EvaluateStatus.Description() + ")";
                    rowtemp.CreateCell(12).SetCellValue(statusName);
                    i++;
                }
            }
            // 写入到客户端   
            var ms = new MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            var dt = DateTime.Now;
            var dateTime = dt.ToString("yyMMddHHmmssfff");
            var fileName = "订单列表" + dateTime + ".xls";
            return File(ms, "application/vnd.ms-excel", fileName);
        }

        /// <summary>
        /// 订单列表金额统计
        /// </summary>
        /// <returns></returns>
        public ActionResult StateOrderList()
        {
            var result = new DataJsonResult();
            //取查询条件
            var orderNo = Request.Get("OrderNo");
            var checkOrderNo = string.IsNullOrWhiteSpace(orderNo);

            var consignee = Request.Get("Consignee");
            var checkConsignee = string.IsNullOrWhiteSpace(consignee);

            var memberName = Request.Get("MemberName");
            var checkMemberName = string.IsNullOrWhiteSpace(memberName);

            var orderStatus = Request.Get("OrderStatus");
            var checkOrderStatus = string.IsNullOrWhiteSpace(orderStatus);
            var orderStatusInt = orderStatus.To<int>();

            var refundStatus = Request.Get("RefundStatus");
            var checkRefundStatus = string.IsNullOrWhiteSpace(refundStatus);
            var refundStatusInt = refundStatus.To<int>();

            var payStatus = Request.Get("PayStatus");
            var checkPayStatus = string.IsNullOrWhiteSpace(payStatus);
            var payStatusInt = payStatus.To<int>();

            var shippingStatus = Request.Get("ShippingStatus");
            var checkShippingStatus = string.IsNullOrWhiteSpace(shippingStatus);
            var shippingStatusInt = shippingStatus.To<int>();

            var createTimeBegin = Request.Get("CreateTimeBegin");
            var checkCreateTimeBegin = string.IsNullOrWhiteSpace(createTimeBegin);
            var createTimeBeginTime = createTimeBegin.To<DateTime>().DayZero();

            var createTimeEnd = Request.Get("CreateTimeEnd");
            var checkCreateTimeEnd = string.IsNullOrWhiteSpace(createTimeEnd);
            var createTimeEndTime = createTimeEnd.To<DateTime>().DayEnd();

            var paymentId = Request.Get("PaymentId");
            var checkPaymentId = string.IsNullOrWhiteSpace(paymentId);

            Expression<Func<Order, bool>> expression =
                l => (checkOrderNo || l.OrderNo.Contains(orderNo)) &&
                     (checkConsignee || l.Consignee.Contains(consignee)) &&
                     (checkMemberName || l.MemberName.Contains(memberName)) &&
                     l.OrderStatus != OrderStatus.Deleted &&
                     (checkOrderStatus || (int)l.OrderStatus == orderStatusInt) &&
                     (checkRefundStatus || (int)l.RefundStatus == refundStatusInt) &&
                     (checkPayStatus || (int)l.PayStatus == payStatusInt) &&
                     (checkPaymentId || l.PaymentId.ToString() == paymentId) &&
                     (checkShippingStatus || (int)l.ShippingStatus == shippingStatusInt) &&
                     (checkCreateTimeBegin || l.CreateTime >= createTimeBeginTime) &&
                     (checkCreateTimeEnd || l.CreateTime <= createTimeEndTime);


            //统计
            var totalOrderAmount = 0M;
            var totalPayFee = 0M;
            var totalRefundFee = 0M;
            var totalGoodsAmount = 0M;
            var totalCouponMoney = 0M;
            var totalIntegralMoney = 0M;
            var totalShippingFee = 0M;
            using (var dbContext = new OrderProcessDbContext())
            {
                var query = dbContext.Orders.Where(expression);
                totalOrderAmount = query.Sum(me => (decimal?)me.OrderAmount) ?? 0;
                totalPayFee = query.Sum(me => (decimal?)me.PayFee) ?? 0;
                totalRefundFee = query.Sum(me => (decimal?)me.RefundFee) ?? 0;
                totalGoodsAmount = query.Sum(me => (decimal?)me.GoodsAmount) ?? 0;
                totalCouponMoney = query.Sum(me => (decimal?)me.CouponMoney) ?? 0;
                totalIntegralMoney = query.Sum(me => (decimal?)me.IntegralMoney) ?? 0;
                totalShippingFee = query.Sum(me => (decimal?)me.ShippingFee) ?? 0;

            }
            result.Data = new
            {
                TotalOrderAmount = totalOrderAmount,
                TotalPayFee = totalPayFee,
                TotalRefundFee = totalRefundFee,
                TotalGoodsAmount = totalGoodsAmount,
                TotalCouponMoney = totalCouponMoney,
                TotalIntegralMoney = totalIntegralMoney,
                TotalShippingFee = totalShippingFee

            };
            return Json(result);
        }


        /// <summary>
        /// 修改收货地址
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="consignee"></param>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="district"></param>
        /// <param name="street"></param>
        /// <param name="address"></param>
        /// <param name="pCDS"></param>
        /// <returns></returns>
        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageOrderKey })]
        public ActionResult SaveAddress(Guid orderId, string consignee, string tel, string province, string city, string district, string street, string address, string pCDS)
        {
            var result = new DataJsonResult();
            if (string.IsNullOrWhiteSpace(consignee) || string.IsNullOrWhiteSpace(province)
                || string.IsNullOrWhiteSpace(city) || string.IsNullOrWhiteSpace(district) ||
                string.IsNullOrWhiteSpace(street)
                || string.IsNullOrWhiteSpace(address) || string.IsNullOrWhiteSpace(pCDS))
                result.ErrorMessage = "收货地址不完整，无法保存";
            else
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    var order = _currencyService.GetSingleById<Order>(orderId);
                    order.Consignee = consignee;
                    order.Tel = tel;
                    order.Province = province;
                    order.City = city;
                    order.District = district;
                    order.Street = street;
                    order.PCDS = pCDS;
                    order.Address = address;
                    _currencyService.Update(order);

                    var action = new OrderAction
                    {
                        Id = KeyGenerator.GetGuidKey(),
                        OrderId = orderId,
                        Memo = "修改收货地址",
                        CreateTime = DateTime.Now,
                        OrderStatus = order.OrderStatus,
                        PayStatus = order.PayStatus,
                        ShippingStatus = order.ShippingStatus,
                        EvaluateStatus = order.EvaluateStatus,
                        UserId = _userContainer.CurrentUser?.Id ?? Guid.Empty.ToString(),
                        UserName = _userContainer.CurrentUser?.UserName ?? "系统"
                    };
                    if (!_currencyService.Create(action))
                        result.ErrorMessage = "修改地址失败";
                    else
                        scope.Complete();
                }
            }
            return Json(result);
        }

        /// <summary>
        /// 修改运费
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="shippingFee"></param>
        /// <returns></returns>
        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageOrderKey })]
        public ActionResult ChangeShippingFee(Guid orderId, decimal shippingFee)
        {
            var result = new DataJsonResult();
            try
            {
                var order = _currencyService.GetSingleById<Order>(orderId);
                if (order.OrderStatus != OrderStatus.PendingPayment)
                    throw new Exception("只有待付款状态可以修改运费");

                var oldFee = order.ShippingFee;

                order.ShippingFee = shippingFee;
                order.OrderAmount = order.OrderAmount + (shippingFee - oldFee);
                order.PayFee = order.PayFee + (shippingFee - oldFee);
                if (!_currencyService.Update(order))
                    throw new Exception("修改运费失败");

                var action = new OrderAction
                {
                    Id = KeyGenerator.GetGuidKey(),
                    OrderId = orderId,
                    Memo = $"修改运费{oldFee}改成{shippingFee}",
                    CreateTime = DateTime.Now,
                    OrderStatus = order.OrderStatus,
                    PayStatus = order.PayStatus,
                    ShippingStatus = order.ShippingStatus,
                    EvaluateStatus = order.EvaluateStatus,
                    UserId = _userContainer.CurrentUser?.Id ?? Guid.Empty.ToString(),
                    UserName = _userContainer.CurrentUser?.UserName ?? "系统"
                };
                if (!_currencyService.Create(action))
                    throw new Exception("修改运费添加记录失败");
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }
            return Json(result);
        }
    }
}