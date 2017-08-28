using System.ComponentModel;

namespace AutoSendOrders.Models
{
    public enum PayStatus
    {
        /// <summary>
        /// 未付款
        /// </summary>
        [Description("未付款")]
        Unpaid = 0,

        /// <summary>
        /// 付款中
        /// </summary>
        [Description("付款中")]
        Paying = 1,

        /// <summary>
        /// 已付款
        /// </summary>
        [Description("已付款")]
        Paid = 2
    }
}