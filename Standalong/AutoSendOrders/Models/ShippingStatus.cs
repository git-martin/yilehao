using System.ComponentModel;

namespace AutoSendOrders.Models
{
    public enum ShippingStatus
    {
        /// <summary>
        /// 未发货
        /// </summary>
        [Description("未发货")]
        Unshipped = 0,

        /// <summary>
        /// 已发货
        /// </summary>
        [Description("已发货")]
        Shipped = 1
    }
}