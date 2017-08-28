using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.FileSystems.Media;
using BntWeb.Mall.Models;
using BntWeb.Mall.ViewModels;
using BntWeb.Mvc;
using BntWeb.Security;
using BntWeb.Utility.Extensions;
using BntWeb.Web.Extensions;

namespace BntWeb.Mall.Controllers
{
    public class GoodsBrandController : Controller
    {
        private readonly ICurrencyService _currencyService;
        private readonly IStorageFileService _storageFileService;
        private const string Logo = "Logo";

        public GoodsBrandController(ICurrencyService currencyService, IStorageFileService storageFileService)
        {
            _currencyService = currencyService;
            _storageFileService = storageFileService;
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageGoodsBrandKey })]
        public ActionResult List()
        {
            return View();
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageGoodsBrandKey })]
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
            var list = _currencyService.GetListPaged<GoodsBrand>(pageIndex, pageSize, out totalCount, new OrderModelField { PropertyName = sortColumn, IsDesc = isDesc });

            result.data = list;
            result.recordsTotal = totalCount;
            result.recordsFiltered = totalCount;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageGoodsBrandKey })]
        public ActionResult Edit(Guid? id = null)
        {
            GoodsBrand goodsBrand = null;
            if (id != null && id != Guid.Empty)
            {
                goodsBrand = _currencyService.GetSingleById<GoodsBrand>(id);
                if (goodsBrand != null) { }
            }

            if (goodsBrand == null)
            {
                goodsBrand = new GoodsBrand
                {
                    Id = KeyGenerator.GetGuidKey()
                };
            }
            return View(goodsBrand);
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageGoodsBrandKey })]
        public ActionResult EditOnPost(GoodsBrand postGoodsBrand)
        {
            var goodsBrand = _currencyService.GetSingleById<GoodsBrand>(postGoodsBrand.Id);
            if (goodsBrand == null)
            {
                _currencyService.Create(postGoodsBrand);
            }
            else
            {
                _currencyService.Update(postGoodsBrand);
            }
            //处理主图图片关联
            _storageFileService.ReplaceFile(postGoodsBrand.Id, MallModule.Key, MallModule.DisplayName, postGoodsBrand.Logo.ToGuid(), Logo);

            return Json(new DataJsonResult());
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageGoodsTypeKey })]
        public ActionResult Delete(Guid brandId)
        {
            var result = new DataJsonResult();
            _currencyService.DeleteByConditon<GoodsBrand>(t => t.Id.Equals(brandId));
            return Json(result);
        }
    }
}