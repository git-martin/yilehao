using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BntWeb.OrderProcess.Models;

namespace BntWeb.OrderProcess.Services
{
    public interface IRefundService: IDependency
    {
        /// <summary>
        /// 创建订单退款申请
        /// </summary>
        /// <param name="refund"></param>
        void CreateOrderRefund(OrderRefund refund);

        /// <summary>
        /// 撤销退款申请 申请中=>已撤销
        /// </summary>
        /// <param name="refund"></param>
        void RevokeOrderRefund(OrderRefund refund);

        /// <summary>
        /// 退款申请审核
        /// 若为仅退款且审核通过则直接打款
        /// 审核通过 仅退款  直接打款  退款申请退款状态：已完成 ，订单商品退款状态：已退款 ，订单退款状态:根据条件判断
        /// 审核通过 退款并退货  退款申请退款状态：已处理
        /// 审核不通过 退款申请退款状态：已完成 ，订单商品退款状态：未退款 ，订单退款状态:根据条件判断
        /// </summary>
        /// <param name="refund"></param>
        void AuditRefund(OrderRefund refund);

        /// <summary>
        /// 订单退款打款
        /// 只有退款类型为退款仅退货且审核通过的退款申请才有打款操作
        /// </summary>
        /// <param name="refund"></param>
        void PayRefund(OrderRefund refund);

    }
}