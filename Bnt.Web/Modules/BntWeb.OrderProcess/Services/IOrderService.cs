/* 
    ======================================================================== 
        File name：        IOrderService
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/7/6 16:20:02
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using BntWeb.OrderProcess.ApiModels;
using BntWeb.OrderProcess.Models;
using BntWeb.OrderProcess.ViewModels;

namespace BntWeb.OrderProcess.Services
{
    public interface IOrderService : IDependency
    {
        /// <summary>
        /// 提交订单
        /// </summary>
        /// <param name="order"></param>
        /// <param name="orderGoods"></param>
        void SubmitOrder(Order order, List<OrderGoods> orderGoods);

        /// <summary>
        /// 加载订单信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Order Load(Guid orderId);

        /// <summary>
        /// 根据条件获取所有订单信息
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        List<Order> GetList(Expression<Func<Order, bool>> expression);

        /// <summary>
        /// 设置订单物流信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="shippingId">快递方式Id</param>
        /// <param name="shippingName">快递名字</param>
        /// <param name="shippingCode">快递公司编码</param>
        /// <param name="shippingNo">快递单号</param>
        /// <returns></returns>
        bool SetShippingInfo(Guid orderId, Guid shippingId, string shippingName, string shippingCode, string shippingNo);

        /// <summary>
        /// 修改订单状态
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="orderStatus"></param>
        /// <param name="payStatus"></param>
        /// <param name="shippingStatus"></param>
        /// <param name="evaluateStatus"></param>
        /// <param name="memo"></param>
        /// <returns></returns>
        bool ChangeOrderStatus(Guid orderId, OrderStatus orderStatus, PayStatus? payStatus = null, ShippingStatus? shippingStatus = null, EvaluateStatus? evaluateStatus = null, string memo = "");

        /// <summary>
        /// 修改订单产品价格
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="orderGoodsId"></param>
        /// <param name="goodsPrice"></param>
        /// <returns></returns>
        bool ChangePrice(Guid orderId, Guid orderGoodsId, decimal goodsPrice);

        /// <summary>
        /// 分页加载订单数据
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="totalCount"></param>
        /// <param name="orderStatus"></param>
        /// <param name="payStatus"></param>
        /// <param name="shippingStatus"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        List<Order> LoadByPage(string memberId, out int totalCount, OrderStatus? orderStatus = null, PayStatus? payStatus = null,
            ShippingStatus? shippingStatus = null, int pageIndex = 1, int pageSize = 10);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="totalCount"></param>
        /// <param name="orderStatus"></param>
        /// <param name="payStatus"></param>
        /// <param name="shippingStatus"></param>
        /// <param name="evaluateStatus"></param>
        /// <param name="refundStatus"></param>
        /// <param name="keywords"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="extentOrderStatus"></param>
        /// <param name="extentRefundStatus"></param>
        /// <returns></returns>
        List<Order> LoadByPage(string memberId, out int totalCount, OrderStatus? orderStatus = null,
            PayStatus? payStatus = null, ShippingStatus? shippingStatus = null, EvaluateStatus? evaluateStatus = null, OrderRefundStatus? refundStatus = null, string keywords = "",
            int pageIndex = 1, int pageSize = 10, OrderStatus? extentOrderStatus = null, OrderRefundStatus? extentRefundStatus = null);
        /// <summary>
        /// 订单评价详情
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        List<EvaluateDetailsModel> LoadOrderEvaluateList(Guid orderId);

        /// <summary>
        /// 加载商品和评价数据
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        List<OrderGoodsEvaluateViewModel> LoadOrderGoodsEvaluateList(Guid orderId);

        /// <summary>
        /// 取消超时未付款订单
        /// </summary>
        /// <param name="outTime"></param>
        /// <returns></returns>
        int CancelTimeOutOrder(DateTime outTime);

        /// <summary>
        /// 自动确认收货
        /// </summary>
        /// <param name="outTime"></param>
        /// <returns></returns>
        int CompleteTimeOutOrder(DateTime outTime);

        #region 提醒发货
        /// <summary>
        /// 获取订单最新提醒发货信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        OrderDeliveryReminder GetNewestReminderInfo(Guid orderId, string memberId);
        /// <summary>
        /// 判断当日是否还能提醒发货  订单状态待发货时使用
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        bool CheckTodayCanRemind(Guid orderId, string memberId);

        #endregion
    }
}