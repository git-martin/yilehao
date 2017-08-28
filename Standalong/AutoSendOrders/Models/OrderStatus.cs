using System.ComponentModel;

namespace AutoSendOrders.Models
{
    public enum OrderStatus
    {
        /// <summary>
        /// 已删除
        /// </summary>
        [Description("已删除")]
        Deleted = -1,

        /// <summary>
        /// 待付款
        /// </summary>
        [Description("待付款")]
        PendingPayment = 0,

        /// <summary>
        /// 待发货
        /// </summary>
        [Description("待发货")]
        WaitingForDelivery = 1,

        /// <summary>
        /// 待收货
        /// </summary>
        [Description("待收货")]
        WaitingForReceiving = 2,

        /// <summary>
        /// 已完成
        /// </summary>
        [Description("已完成")]
        Completed = 3,

        /// <summary>
        /// 已关闭
        /// </summary>
        [Description("已关闭")]
        Closed = 4

    }
}