
/* Models Code Auto-Generation 
    ======================================================================== 
        File name：		Orders
        Module:			
        Author：		罗嗣宝
        Create Time：		2016/7/6 16:49:29
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/

using System.ComponentModel;

namespace AutoSendOrders.Models
{
    /// <summary>
    /// 退款状态
    /// </summary>
    public enum OrderRefundStatus
    {
        /// <summary>
        /// 未退款
        /// </summary>
        [Description("未退款")]
        NoRefund = 0,

        /// <summary>
        /// 退款中
        /// </summary>
        [Description("退款中")]
        Refunding = 1,

        /// <summary>
        /// 已退款
        /// </summary>
        [Description("已退款")]
        Refunded = 2
    }
}