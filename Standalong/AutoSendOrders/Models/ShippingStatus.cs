using System.ComponentModel;

namespace AutoSendOrders.Models
{
    public enum ShippingStatus
    {
        /// <summary>
        /// δ����
        /// </summary>
        [Description("δ����")]
        Unshipped = 0,

        /// <summary>
        /// �ѷ���
        /// </summary>
        [Description("�ѷ���")]
        Shipped = 1
    }
}