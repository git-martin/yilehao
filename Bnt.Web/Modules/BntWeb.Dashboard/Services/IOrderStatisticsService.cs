/* 
    ======================================================================== 
        File name：        IOrderStattisticService
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/8/23 17:13:38
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BntWeb.OrderProcess.Models;

namespace BntWeb.Dashboard.Services
{
    public interface IOrderStatisticsService : IDependency
    {
        /// <summary>
        /// 获取今天的销售额
        /// </summary>
        /// <returns></returns>
        decimal SalesAmountToday();

        /// <summary>
        /// 获取今天订单量
        /// </summary>
        /// <returns></returns>
        int SalesVolumeToday();

        /// <summary>
        /// 获取今天新增用户
        /// </summary>
        /// <returns></returns>
        int NewMembersToday();

        /// <summary>
        /// 统计订单数量
        /// </summary>
        /// <param name="orderStatus"></param>
        /// <returns></returns>
        int CountOrder(OrderStatus orderStatus);

        /// <summary>
        /// 统计退款订单
        /// </summary>
        /// <returns></returns>
        int CountOrder(OrderRefundStatus refundStatus);

        /// <summary>
        /// 库存警告商品数量
        /// </summary>
        /// <returns></returns>
        int InventoryWarning();

        /// <summary>
        /// 销量统计
        /// </summary>
        /// <returns></returns>
        List<StatisticSalesResult> StatisticSales();

        /// <summary>
        /// 会员统计
        /// </summary>
        /// <returns></returns>
        List<StatisticMembersResult> StatisticMembers();

        /// <summary>
        /// 催发货次数
        /// </summary>
        /// <returns></returns>
        int DeliveryRemindersCount();

        int CountRefundOrder(RefundType type);
        int CountUnDoEvalute();
    }
}

public class StatisticSalesResult
{
    public string Month { get; set; }

    public int SalesVolume { get; set; }

    public decimal SalesAmount { get; set; }
}

public class StatisticMembersResult
{
    public string Month { get; set; }
    
    public decimal Amount { get; set; }
}