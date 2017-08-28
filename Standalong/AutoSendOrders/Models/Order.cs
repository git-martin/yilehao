using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoSendOrders.Models
{
    /// <summary>
    /// ʵ�壺Orders
    /// </summary>
    [Table("Bnt_Orders")]
    public class Order
    {
        /// <summary>
        /// 
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// �������
        /// </summary>
        [MaxLength(30)]
        public string OrderNo { get; set; }

        /// <summary>
        /// �µ���Id
        /// </summary>
        [MaxLength(36)]
        public string MemberId { get; set; }

        /// <summary>
        /// �µ����û���
        /// </summary>
        [MaxLength(256)]
        public string MemberName { get; set; }

        /// <summary>
        /// ����״̬
        /// </summary>
        public OrderStatus OrderStatus { get; set; }

        /// <summary>
        /// ����״̬
        /// </summary>
        public ShippingStatus ShippingStatus { get; set; }

        /// <summary>
        /// ֧��״̬
        /// </summary>
        public PayStatus PayStatus { get; set; }

        /// <summary>
        /// ����״̬
        /// </summary>
        public EvaluateStatus EvaluateStatus { get; set; }
        /// <summary>
        /// �˿�״̬
        /// </summary>
        public OrderRefundStatus RefundStatus { get; set; }

        /// <summary>
        /// �ջ���
        /// </summary>
        [MaxLength(60)]
        public string Consignee { get; set; }

        /// <summary>
        /// ʡ�ݱ��
        /// </summary>
        [MaxLength(10)]
        public string Province { get; set; }

        /// <summary>
        /// ���б��
        /// </summary>
        [MaxLength(10)]
        public string City { get; set; }

        /// <summary>
        /// ���ر��
        /// </summary>
        [MaxLength(10)]
        public string District { get; set; }

        /// <summary>
        /// �ֵ�/������
        /// </summary>
        [MaxLength(10)]
        public string Street { get; set; }

        /// <summary>
        /// �ļ�������ȫ�Ƶĺϼ�
        /// </summary>
        [MaxLength(200)]
        public string PCDS { get; set; }

        /// <summary>
        /// ��ϸ��ַ
        /// </summary>
        [MaxLength(200)]
        public string Address { get; set; }

        /// <summary>
        /// �绰
        /// </summary>
        [MaxLength(100)]
        public string Tel { get; set; }

        /// <summary>
        /// ����ͻ�ʱ��
        /// </summary>
        [MaxLength(100)]
        public string BestTime { get; set; }

        /// <summary>
        /// �Ƿ���Ҫ����
        /// </summary>
        public bool NeedShipping { get; set; }

        /// <summary>
        /// ������ʽId
        /// </summary>
        public Guid ShippingId { get; set; }

        /// <summary>
        /// ������ʽ����
        /// </summary>
        [MaxLength(120)]
        public string ShippingName { get; set; }

        /// <summary>
        /// ������˾����
        /// </summary>
        [MaxLength(120)]
        public string ShippingCode { get; set; }

        /// <summary>
        /// ��ݵ���
        /// </summary>
        [MaxLength(30)]
        public string ShippingNo { get; set; }

        /// <summary>
        /// �Ƿ�����֧��
        /// </summary>
        public bool PayOnline { get; set; }

        /// <summary>
        /// ֧����ʽId
        /// </summary>
        public Guid PaymentId { get; set; }

        /// <summary>
        /// ֧����ʽ����
        /// </summary>
        [MaxLength(120)]
        public string PaymentName { get; set; }

        /// <summary>
        /// ��Ʒ�ܼ�
        /// </summary>
        public decimal GoodsAmount { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        public decimal ShippingFee { get; set; }

        /// <summary>
        /// �����ܶ�=��Ʒ�ܼ�+��������
        /// </summary>
        public decimal OrderAmount { get; set; }

        /// <summary>
        /// ʹ�û�������
        /// </summary>
        public int Integral { get; set; }

        /// <summary>
        /// �����۵ֽ��
        /// </summary>
        public decimal IntegralMoney { get; set; }


        /// <summary>
        /// �Ż�ȯ�Żݽ��
        /// </summary>
        public decimal CouponMoney { set; get; }

        /// <summary>
        /// ʵ���ܶ�=�����ܶ�-�ۿ�-�۵�
        /// </summary>
        public decimal PayFee { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        public decimal BalancePay { get; set; }

        /// <summary>
        /// δ֧���ܶ�=ʵ���ܶ�-����
        /// </summary>
        public decimal UnpayFee { get; set; }

        /// <summary>
        /// �µ�ʱ��
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// ֧��ʱ��
        /// </summary>
        public DateTime? PayTime { get; set; }

        /// <summary>
        /// ����ʱ��
        /// </summary>
        public DateTime? ShippingTime { get; set; }

        /// <summary>
        /// ������ע
        /// </summary>
        [MaxLength(200)]
        public string Memo { get; set; }

        /// <summary>
        /// �˿���
        /// </summary>
        public decimal RefundFee { get; set; }

        [MaxLength(50)]
        public string ModuleKey { get; set; }


        [MaxLength(50)]
        public string ModuleName { get; set; }


        [MaxLength(20)]
        public string SourceType { get; set; }


        public virtual List<OrderGoods> OrderGoods { get; set; }

    }
}