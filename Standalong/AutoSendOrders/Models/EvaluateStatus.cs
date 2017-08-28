using System.ComponentModel;

namespace AutoSendOrders.Models
{
    public enum EvaluateStatus
    {
        /// <summary>
        /// 未评价
        /// </summary>
        [Description("未评价")]
        NotEvaluated = 0,

        /// <summary>
        /// 已评价
        /// </summary>
        [Description("已评价")]
        Evaluated = 1,

        /// <summary>
        /// 已回复
        /// </summary>
        [Description("已回复")]
        Replied = 2
    }
}