using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using BntWeb.Config.Models;
using BntWeb.Data.Services;
using BntWeb.FileSystems.Media;
using BntWeb.Logging;
using BntWeb.Mall.Models;
using BntWeb.Mall.Services;
using BntWeb.Mvc;
using BntWeb.Security;
using BntWeb.Services;
using BntWeb.Utility.Extensions;
using BntWeb.Web.Extensions;

namespace BntWeb.Mall.Controllers
{
    public class GoodsShortageController : Controller
    {
        private readonly ICurrencyService _currencyService;
        private readonly IConfigService _configService;
        private readonly IGoodsService _goodsService;

        public GoodsShortageController(ICurrencyService currencyService, IConfigService configService,IGoodsService goodsService)
        {
            _currencyService = currencyService;
            _configService = configService;
            _goodsService = goodsService;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageGoodsKey })]
        public ActionResult List()
        {
            var categories = _goodsService.GetCategories();
            if (categories == null || categories.Count == 0)
                throw new BntWebCoreException("商品分类异常");
            ViewBag.CategoriesJson = categories.Select(me => new { id = me.Id, pId = me.ParentId, name = me.Name }).ToList().ToJson();
            return View();
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageGoodsKey })]
        public ActionResult ListOnPage()
        {
            var result = new DataTableJsonResult();

            //取参数值
            int draw, pageIndex, pageSize, totalCount;
            string sortColumn;
            bool isDesc;
            Request.GetDatatableParameters(out draw, out pageIndex, out pageSize, out sortColumn, out isDesc);
            result.draw = draw;

            //取查询条件
            var name = Request.Get("extra_search[Name]");
            var checkName = string.IsNullOrWhiteSpace(name);

            var goodsNo = Request.Get("extra_search[GoodsNo]");
            var checkGoodsNo = string.IsNullOrWhiteSpace(goodsNo);

            var status = Request.Get("extra_search[Status]");
            var checkStatus = string.IsNullOrWhiteSpace(status);

            var categoryId = Request.Get("extra_search[CategoryId]");
            var checkCategoryId = string.IsNullOrWhiteSpace(categoryId);

            var config = _configService.Get<SystemConfig>();

            Expression<Func<Models.GoodsCategoryNameView, bool>> expression =
                l => (checkName || l.Name.ToString().Equals(name, StringComparison.OrdinalIgnoreCase)) &&
                     (checkGoodsNo || l.GoodsNo.Contains(goodsNo)) &&
                     (checkStatus || ((int)l.Status).ToString().Equals(status)) &&
                     (checkCategoryId||l.MergerId.Contains(categoryId))&&
                     l.Stock <= config.StockWarning &&
                     l.Status != GoodsStatus.Delete;

            //分页查询
            var list = _currencyService.GetListPaged<GoodsCategoryNameView>(pageIndex, pageSize, expression, out totalCount, new OrderModelField { PropertyName = sortColumn, IsDesc = isDesc });

            result.data = list;
            result.recordsTotal = totalCount;
            result.recordsFiltered = totalCount;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #region 缺货商品导出
        [AdminAuthorize(PermissionsArray = new[] { Permissions.GoodsToExeclKey })]
        public FileResult GoodsShortageToExecl()
        {
            //取查询条件
            var name = Request.Get("Name");
            var checkName = string.IsNullOrWhiteSpace(name);

            var goodsNo = Request.Get("GoodsNo");
            var checkGoodsNo = string.IsNullOrWhiteSpace(goodsNo);

            var status = Request.Get("Status");
            var checkStatus = string.IsNullOrWhiteSpace(status);

            var categoryId = Request.Get("CategoryId");
            var checkCategoryId = string.IsNullOrWhiteSpace(categoryId);

            var config = _configService.Get<SystemConfig>();
            Expression<Func<Models.GoodsCategoryNameView, bool>> expression =
                l => (checkName || l.Name.ToString().Equals(name, StringComparison.OrdinalIgnoreCase)) &&
                     (checkGoodsNo || l.GoodsNo.Contains(goodsNo)) &&
                     (checkStatus || ((int)l.Status).ToString().Equals(status)) &&
                     (checkCategoryId || l.MergerId.Contains(categoryId)) &&
                     l.Stock <= config.StockWarning &&
                     l.Status != GoodsStatus.Delete;

            //分页查询
            var goods = _currencyService.GetList<GoodsCategoryNameView>( expression,  new OrderModelField { PropertyName = "CreateTime", IsDesc = true });

            //创建Excel文件的对象  
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();

            //添加一个sheet  
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");
            if (goods.Any())
            {
                //给sheet1添加第一行的头部标题  
                NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
                row1.CreateCell(0).SetCellValue("商品名称");
                row1.CreateCell(1).SetCellValue("货号");
                row1.CreateCell(2).SetCellValue("价格");
                row1.CreateCell(3).SetCellValue("原价");
                row1.CreateCell(4).SetCellValue("状态");
                row1.CreateCell(5).SetCellValue("库存");
                row1.CreateCell(6).SetCellValue("销量");
                row1.CreateCell(7).SetCellValue("新品");
                row1.CreateCell(8).SetCellValue("热卖");
                row1.CreateCell(9).SetCellValue("精品");
                var i = 0;
                foreach (var good in goods)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(good.Name);
                    rowtemp.CreateCell(1).SetCellValue(good.GoodsNo);
                    rowtemp.CreateCell(2).SetCellValue(good.ShopPrice.ToString(CultureInfo.InvariantCulture));
                    rowtemp.CreateCell(3).SetCellValue(good.OriginalPrice.ToString(CultureInfo.InvariantCulture));
                    rowtemp.CreateCell(4).SetCellValue(good.Status.Description());
                    rowtemp.CreateCell(5).SetCellValue(good.Stock);
                    rowtemp.CreateCell(6).SetCellValue(good.SalesVolume);
                    rowtemp.CreateCell(7)
                        .SetCellValue(good.IsNew ? "是" : "否");
                    rowtemp.CreateCell(8).SetCellValue(good.IsHot ? "是" : "否");
                    rowtemp.CreateCell(9).SetCellValue(good.IsBest ? "是" : "否");
                    i++;
                }
            }
            // 写入到客户端   
            var ms = new MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            var dt = DateTime.Now;
            var dateTime = dt.ToString("yyMMddHHmmssfff");
            var fileName = "缺货商品列表" + dateTime + ".xls";
            return File(ms, "application/vnd.ms-excel", fileName);
        }

        #endregion
    }
}