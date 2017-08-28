using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.FileSystems.Media;
using BntWeb.Logging;
using BntWeb.Mall.Models;
using BntWeb.Mall.Services;
using BntWeb.Mall.ViewModels;
using BntWeb.Mvc;
using BntWeb.Security;
using BntWeb.Services;
using BntWeb.Utility.Extensions;
using BntWeb.Web.Extensions;

namespace BntWeb.Mall.Controllers
{
    public class GoodsRecycleController : Controller
    {
        private readonly ICurrencyService _currencyService;
        private readonly IGoodsService _goodsService;

        public GoodsRecycleController(ICurrencyService currencyService, IGoodsService goodsService)
        {
            _currencyService = currencyService;
            _goodsService = goodsService;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageGoodsKey })]
        public ActionResult List()
        {
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



            Expression<Func<Models.Goods, bool>> expression =
                l => (checkName || l.Name.ToString().Contains(name)) &&
                     (checkGoodsNo || l.GoodsNo.Contains(goodsNo)) &&
                     l.Status==GoodsStatus.Delete ;

            //分页查询
            var list = _currencyService.GetListPaged<Goods>(pageIndex, pageSize, expression, out totalCount, new OrderModelField { PropertyName = sortColumn, IsDesc = isDesc });

            result.data = list;
            result.recordsTotal = totalCount;
            result.recordsFiltered = totalCount;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageGoodsKey })]
        public ActionResult Delete(Guid goodsId)
        {
            var result = new DataJsonResult();
            if (!_goodsService.DeleteGoods(goodsId))
            {
                result.ErrorMessage = "未知异常，删除失败";
            }

            return Json(result);
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="goodsIds"></param>
        /// <returns></returns>
        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageGoodsKey })]
        public ActionResult BatchDelete(List<Guid> goodsIds)
        {
            var result = new DataJsonResult();
            if (!_goodsService.BatchDeleteGoods(goodsIds))
            {
                result.ErrorMessage = "未知异常，删除失败";
            }

            return Json(result);
        }

        /// <summary>
        /// 还原
        /// </summary>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        [AdminAuthorize(PermissionsArray = new[] {Permissions.ManageGoodsKey})]
        public ActionResult Restore(Guid goodsId)
        {
            var result = new DataJsonResult();
            if (!_goodsService.RestoreGoods(goodsId))
            {
                result.ErrorMessage = "未知异常，还原失败";
            }

            return Json(result);
        }
    }
}