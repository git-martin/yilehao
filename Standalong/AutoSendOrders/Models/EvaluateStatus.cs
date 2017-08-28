using System.ComponentModel;

namespace AutoSendOrders.Models
{
    public enum EvaluateStatus
    {
        /// <summary>
        /// δ����
        /// </summary>
        [Description("δ����")]
        NotEvaluated = 0,

        /// <summary>
        /// ������
        /// </summary>
        [Description("������")]
        Evaluated = 1,

        /// <summary>
        /// �ѻظ�
        /// </summary>
        [Description("�ѻظ�")]
        Replied = 2
    }
}