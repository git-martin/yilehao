using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using BntWeb.Config.Models;
//using BntWeb.Caching;
using BntWeb.Dashboard.Services;
using BntWeb.Dashboard.ViewModels;
using BntWeb.Data.Services;
using BntWeb.Logging;
using BntWeb.Mvc;
using BntWeb.OrderProcess;
using BntWeb.OrderProcess.Models;
using BntWeb.Security;
using BntWeb.Services;
using BntWeb.Utility.Extensions;
using BntWeb.Web.Extensions;

namespace BntWeb.Dashboard.Controllers
{
    [AdminAuthorize]
    public class StatisticsController : Controller
    {
        //private readonly ICacheManager _cacheManager;
        private readonly IStatisticalService _statisticalService;
        private readonly IConfigService _configService;
        public StatisticsController(IStatisticalService statisticalService, IConfigService configService)
        {
            _statisticalService = statisticalService;
            _configService = configService;
            //_cacheManager = cacheManager;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        //[AdminAuthorize]
        public ActionResult List()
        {
            return View();
        }


        public ActionResult ListOnPage(StatisticalView model)
        {
            var result = new DataTableJsonResult();
            List<StatisticalReports> statisticalReports = new List<StatisticalReports>();


            int draw, pageIndex = 1, pageSize = 10000, totalCount = 1;
            string sortColumn;
            bool isDesc;
            Request.GetDatatableParameters(out draw, out pageIndex, out pageSize, out sortColumn, out isDesc);
            result.draw = draw;

            //取查询条件
            var year = Request.Get("extra_search[Year]");

            var statisticalway = Request.Get("extra_search[Statisticalway]");
            var statisticalwayint = statisticalway.To<int>();

            var startTime = Request.Get("extra_search[StartTime]");
            DateTime? createTimestartTime = null;
            if (startTime != "")
                createTimestartTime = startTime.To<DateTime>().DayZero();


            var endTime = Request.Get("extra_search[EndTime]");
            DateTime? createTimeendTime = null;
            if (endTime != "")
                createTimeendTime = endTime.To<DateTime>().DayEnd();

            var province = Request.Get("extra_search[State_Province]");
            var city = Request.Get("extra_search[State_City]");

            switch (statisticalwayint)
            {
                case 1://年
                    {
                        statisticalReports = _statisticalService.YearStatisticalReports();
                    }
                    break;
                case 2://月
                    {
                        statisticalReports = _statisticalService.MonthStatisticalReports(year);
                    }
                    break;
                case 3://区间
                    {
                        var statistical = _statisticalService.StatisticalReports(createTimestartTime, createTimeendTime);
                        statisticalReports.Add(statistical);
                    }
                    break;
                case 4://地区统计
                    {
                        statisticalReports = _statisticalService.AreaOrderStatisticalReport(province, city);
                    }
                    break;
                default:
                    break;
            }
            result.data = statisticalReports;
            result.recordsTotal = totalCount;
            result.recordsFiltered = totalCount;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MemberList()
        {
            return View();
        }
        public ActionResult MemberListOnPage(StatisticalView model)
        {
            var result = new DataTableJsonResult();
            List<StatisticalReports> statisticalReports = new List<StatisticalReports>();


            int draw, pageIndex = 1, pageSize = 10000, totalCount = 1;
            string sortColumn;
            bool isDesc;
            Request.GetDatatableParameters(out draw, out pageIndex, out pageSize, out sortColumn, out isDesc);
            result.draw = draw;

            //取查询条件
            var year = Request.Get("extra_search[Year]");

            var statisticalway = Request.Get("extra_search[Statisticalway]");
            var statisticalwayint = statisticalway.To<int>();

            var startTime = Request.Get("extra_search[StartTime]");
            DateTime? createTimestartTime = null;
            if (startTime != "")
                createTimestartTime = startTime.To<DateTime>().DayZero();

            var endTime = Request.Get("extra_search[EndTime]");
            DateTime? createTimeendTime = null;
            if (endTime != "")
                createTimeendTime = endTime.To<DateTime>().DayEnd();

            switch (statisticalwayint)
            {
                case 1://年
                    {
                        statisticalReports = _statisticalService.YearStatisticalReportsMember();
                    }
                    break;
                case 2://月
                    {
                        statisticalReports = _statisticalService.MonthStatisticalReportsMember(year);
                    }
                    break;
                case 3://区间
                    {
                        var statistical = _statisticalService.StatisticalReportsMember(createTimestartTime, createTimeendTime);
                        statisticalReports.Add(statistical);
                    }
                    break;
                default:
                    break;
            }
            result.data = statisticalReports;
            result.recordsTotal = totalCount;
            result.recordsFiltered = totalCount;
            return Json(result, JsonRequestBehavior.AllowGet);
        }


    }
}