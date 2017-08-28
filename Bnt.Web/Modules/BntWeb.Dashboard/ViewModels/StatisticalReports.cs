using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BntWeb.Dashboard.ViewModels
{
    public class StatisticalReports
    {
        /// <summary>
        /// 时间/统计分组类型
        /// </summary>
        public string Times { get; set; }
        /// <summary>
        /// 订单数
        /// </summary>
        public int OrderNumber { get; set; }
        /// <summary>
        /// 销售额
        /// </summary>
        public decimal Sales { get; set; }
        /// <summary>
        /// 退款金额
        /// </summary>
        public decimal Refund { set; get; }
        /// <summary>
        /// 会员数
        /// </summary>
        public int MemberNumber { get; set; }
        /// <summary>
        /// 新增会员数
        /// </summary>
        public int NewMemberNumber { get; set; }
    }
    public class StatisticalView
    {
        /// <summary>
        /// 时间
        /// </summary>
        public string Year { get; set; }
        /// <summary>
        /// 订单数
        /// </summary>
        public int Statisticalway { get; set; }
        /// <summary>
        /// 销售额
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 会员数
        /// </summary>
        public DateTime EndTime { get; set; }

    }


    public class MemberSalesReportModel
    {
        public string Id { set; get; }
        public string NickName { set; get; }
        public string UserName { set; get; }
        public string PhoneNumber { set; get; }
        public string RegionName { set; get; }
        public string Address { set; get; }
        public string InvitationCode { set; get; }
        public int Level { set; get; }
        public decimal MinDiscountRate { set; get; }
        public int MemberType { set; get; }
        /// <summary>
        /// 固定下滑额度
        /// </summary>
        public decimal DescendLimitAmount { set; get; }
        /// <summary>
        /// 本月采购金额
        /// </summary>
        public decimal ThisMonthAmount { set; get; }
        /// <summary>
        /// 上月采购金额
        /// </summary>
        public decimal LastMonthAmount { set; get; }
        /// <summary>
        /// 下滑金额，本月-上月
        /// </summary>
        public decimal DescendAmount { set; get; }

        /// <summary>
        /// 下滑比例(本月-上月)/上月
        /// </summary>
        public decimal DescendRate { set; get; }


    }
}