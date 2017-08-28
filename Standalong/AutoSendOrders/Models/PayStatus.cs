using System.ComponentModel;

namespace AutoSendOrders.Models
{
    public enum PayStatus
    {
        /// <summary>
        /// δ����
        /// </summary>
        [Description("δ����")]
        Unpaid = 0,

        /// <summary>
        /// ������
        /// </summary>
        [Description("������")]
        Paying = 1,

        /// <summary>
        /// �Ѹ���
        /// </summary>
        [Description("�Ѹ���")]
        Paid = 2
    }
}