using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.Mall.Models;
using BntWeb.Mall.ViewModels;
using BntWeb.Mvc;
using BntWeb.Security;
using BntWeb.Utility.Extensions;
using BntWeb.Web.Extensions;

namespace BntWeb.Mall.Controllers
{
    public class GoodsTypeController : Controller
    {
        private readonly ICurrencyService _currencyService;

        public GoodsTypeController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageGoodsTypeKey })]
        public ActionResult List()
        {
            return View();
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageGoodsTypeKey })]
        public ActionResult ListOnPage()
        {
            var result = new DataTableJsonResult();

            //取参数值
            int draw, pageIndex, pageSize, totalCount;
            string sortColumn;
            bool isDesc;
            Request.GetDatatableParameters(out draw, out pageIndex, out pageSize, out sortColumn, out isDesc);
            result.draw = draw;

            //分页查询
            var list = _currencyService.GetListPaged<GoodsType>(pageIndex, pageSize, out totalCount, new OrderModelField { PropertyName = sortColumn, IsDesc = isDesc });

            result.data = list;
            result.recordsTotal = totalCount;
            result.recordsFiltered = totalCount;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageGoodsTypeKey })]
        public ActionResult Edit(Guid? id = null)
        {
            GoodsType goodsType = null;
            if (id != null && id != Guid.Empty)
            {
                goodsType = _currencyService.GetSingleById<GoodsType>(id);
                if (goodsType != null) { }
            }

            if (goodsType == null)
            {
                goodsType = new GoodsType
                {
                    Id = KeyGenerator.GetGuidKey()
                };
            }
            return View(goodsType);
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageGoodsTypeKey })]
        public ActionResult EditOnPost(GoodsType postGoodsType)
        {
            var goodsType = _currencyService.GetSingleById<GoodsType>(postGoodsType.Id);
            if (goodsType == null)
            {
                _currencyService.Create(postGoodsType);
            }
            else
            {
                _currencyService.Update(postGoodsType);
            }
            return Json(new DataJsonResult());
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageGoodsTypeKey })]
        public ActionResult SwitchEnabled(SwitchGoodsTypeViewModel switchGoodsType)
        {
            var result = new DataJsonResult();
            var goodsType = _currencyService.GetSingleById<GoodsType>(switchGoodsType.TypeId);
            if (goodsType == null)
            {
                result.ErrorMessage = "商品类型不存在";
            }
            else
            {
                goodsType.Enabled = switchGoodsType.Enabled;
                _currencyService.Update(goodsType);
            }
            return Json(result);
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageGoodsTypeKey })]
        public ActionResult Delete(Guid typeId)
        {
            var result = new DataJsonResult();
            _currencyService.DeleteByConditon<GoodsType>(t => t.Id.Equals(typeId));
            _currencyService.DeleteByConditon<GoodsAttribute>(t => t.GoodsTypeId.Equals(typeId));
            return Json(result);
        }
    }
}