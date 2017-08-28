using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BntWeb.Dashboard.ViewModels;

namespace BntWeb.Dashboard.Services
{
    public interface IStatisticalService:IDependency
    {
        StatisticalReports StatisticalReports(DateTime? startTime = null, DateTime? endTime = null);
        List<StatisticalReports> YearStatisticalReports();
        List<StatisticalReports> MonthStatisticalReports(string year);
        StatisticalReports StatisticalReportsMember(DateTime? startTime = null, DateTime? endTime = null);
        List<StatisticalReports> YearStatisticalReportsMember();
        List<StatisticalReports> MonthStatisticalReportsMember(string year);

        /// <summary>
        /// 收货区域订单统计报表
        /// </summary>
        /// <returns></returns>
        List<StatisticalReports> AreaOrderStatisticalReport(string province, string city);

    }
}