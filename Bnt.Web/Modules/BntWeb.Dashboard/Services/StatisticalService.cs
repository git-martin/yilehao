using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BntWeb.Config.Models;
using BntWeb.Dashboard.ViewModels;
using BntWeb.OrderProcess.Models;
using BntWeb.Services;

namespace BntWeb.Dashboard.Services
{
    public class StatisticalService : IStatisticalService
    {
        private readonly IConfigService _configService;

        public StatisticalService(IConfigService configService)
        {
            _configService = configService;
        }

        public StatisticalReports StatisticalReports(DateTime? startTime = null, DateTime? endTime = null)
        {
            var checkStartTime = startTime == null;
            var checkEndTime = endTime == null;
            using (var dbContext = new DashboardDbContext())
            {
                var sqlorder = from o in dbContext.Orders
                               where (checkStartTime || o.CreateTime >= startTime) && (checkEndTime || o.CreateTime <= endTime) &&
                                     (o.OrderStatus == OrderStatus.WaitingForDelivery ||
                                      o.OrderStatus == OrderStatus.WaitingForReceiving || o.OrderStatus == OrderStatus.Completed)
                               select o;
                return new StatisticalReports()
                {
                    Times = $"{startTime?.ToString("yyyy/MM/dd")}-{(endTime==null? DateTime.Now.ToString("yyyy/MM/dd"):endTime?.ToString("yyyy/MM/dd"))}",
                    OrderNumber = sqlorder.Count(),
                    Sales = sqlorder.Sum(s => (decimal?)s.PayFee) ?? 0,
                    Refund = sqlorder.Sum(s => (decimal?)s.RefundFee) ?? 0
                };
            }
        }

        public List<StatisticalReports> YearStatisticalReports()
        {
            using (var dbContext = new DashboardDbContext())
            {
                var sqlorder = "SELECT YEAR(CreateTime) as Times ,SUM(PayFee) AS Sales , COUNT(*) AS OrderNumber,SUM(RefundFee) as Refund from bnt_orders WHERE OrderStatus=1 OR OrderStatus=2 OR OrderStatus=3 GROUP BY YEAR(CreateTime)";
                var applyWork = dbContext.Database.SqlQuery<StatisticalReports>(sqlorder).ToList();
                return applyWork;

            }
        }

        public List<StatisticalReports> MonthStatisticalReports(string year)
        {
            using (var dbContext = new DashboardDbContext())
            {
                var sqlorder = $"SELECT MONTH(CreateTime) as Times ,SUM(PayFee) AS Sales , COUNT(*) AS OrderNumber,SUM(RefundFee) as Refund from bnt_orders WHERE YEAR(CreateTime)={year} and (OrderStatus=1 OR OrderStatus=2 OR OrderStatus=3) GROUP BY MONTH(CreateTime)";
                var applyWork = dbContext.Database.SqlQuery<StatisticalReports>(sqlorder).ToList();
                return applyWork;
            }
        }


        public StatisticalReports StatisticalReportsMember(DateTime? startTime = null, DateTime? endTime = null)
        {
            var checkStartTime = startTime == null;
            var checkEndTime = endTime == null;
            using (var dbContext = new DashboardDbContext())
            {
                var sqlnewmember = from m in dbContext.Members
                                   where (checkStartTime || m.CreateTime >= startTime) && (checkEndTime || m.CreateTime <= endTime)
                                   select m.Id;
                var sqlmember = from m in dbContext.Members
                                where (checkEndTime || m.CreateTime <= endTime)
                                select m.Id;
                return new StatisticalReports()
                {
                    Times = $"{startTime?.ToString("yyyy/MM/dd")}-{(endTime == null ? DateTime.Now.ToString("yyyy/MM/dd") : endTime?.ToString("yyyy/MM/dd"))}",
                    //Times = $"{startTime}-{endTime}",
                    NewMemberNumber = sqlnewmember.Count(),
                    MemberNumber = sqlmember.Count()
                };
            }
        }


        public List<StatisticalReports> YearStatisticalReportsMember()
        {
            using (var dbContext = new DashboardDbContext())
            {
                var sqlorder = "select *,(select Sum(NewMemberNumber) from bnt_view_member_state_year where Times<=s.Times) as MemberNumber from bnt_view_member_state_year s";
                var applyWork = dbContext.Database.SqlQuery<StatisticalReports>(sqlorder).ToList();
                return applyWork;

            }
        }

        public List<StatisticalReports> MonthStatisticalReportsMember(string year)
        {
            using (var dbContext = new DashboardDbContext())
            {
                var sqlorder = $"select s.Months as Times ,s.NewMemberNumber,(select Sum(NewMemberNumber) from bnt_view_member_state_month where YearsMonths<=s.YearsMonths ) as MemberNumber from bnt_view_member_state_month s where  s.years={year} ORDER BY Months";
                var applyWork = dbContext.Database.SqlQuery<StatisticalReports>(sqlorder).ToList();
                return applyWork;
            }
        }

        /// <summary>
        /// 收货区域订单统计报表
        /// </summary>
        /// <returns></returns>
        public List<StatisticalReports> AreaOrderStatisticalReport(string province, string city)
        {
            var sql = "";
            if (string.IsNullOrWhiteSpace(province) && string.IsNullOrWhiteSpace(city))
            {
                sql = @"select d.FullName as Times,IFNULL(sum(o.PayFee),0) as Sales,IFNULL(SUM(o.RefundFee),0) as Refund,count(1) as OrderNumber from bnt_orders o
            LEFT JOIN bnt_system_districts d on o.Province=d.Id
            where o.OrderStatus in(1,2,3)
            GROUP BY o.Province;";
            }
            if (!string.IsNullOrWhiteSpace(province) && string.IsNullOrWhiteSpace(city))
            {
                sql = @"select d.FullName as Times,IFNULL(sum(o.PayFee),0) as Sales,IFNULL(SUM(o.RefundFee),0) as Refund,count(1) as OrderNumber from bnt_orders o
            LEFT JOIN bnt_system_districts d on o.City=d.Id
            where o.OrderStatus in(1,2,3) and o.Province='{0}'
            GROUP BY o.City;";
                sql = string.Format(sql, province);
            }
            if (!string.IsNullOrWhiteSpace(province) && !string.IsNullOrWhiteSpace(city))
            {
                sql = @"select d.FullName as Times,IFNULL(sum(o.PayFee),0) as Sales,IFNULL(SUM(o.RefundFee),0) as Refund,count(1) as OrderNumber from bnt_orders o
            LEFT JOIN bnt_system_districts d on o.District=d.Id
            where o.OrderStatus in(1,2,3) and o.City='{0}'
            GROUP BY o.District;";
                sql = string.Format(sql, city);
            }

            using (var dbContext = new DashboardDbContext())
            {
                var report = dbContext.Database.SqlQuery<StatisticalReports>(sql).ToList();
                return report;
            }


        }

    }
}