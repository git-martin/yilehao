using System.ComponentModel;

namespace AutoSendOrders.Models
{
    public enum OrderStatus
    {
        /// <summary>
        /// ��ɾ��
        /// </summary>
        [Description("��ɾ��")]
        Deleted = -1,

        /// <summary>
        /// ������
        /// </summary>
        [Description("������")]
        PendingPayment = 0,

        /// <summary>
        /// ������
        /// </summary>
        [Description("������")]
        WaitingForDelivery = 1,

        /// <summary>
        /// ���ջ�
        /// </summary>
        [Description("���ջ�")]
        WaitingForReceiving = 2,

        /// <summary>
        /// �����
        /// </summary>
        [Description("�����")]
        Completed = 3,

        /// <summary>
        /// �ѹر�
        /// </summary>
        [Description("�ѹر�")]
        Closed = 4

    }
}