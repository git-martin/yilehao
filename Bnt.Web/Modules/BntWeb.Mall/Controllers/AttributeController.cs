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
    public class AttributeController : Controller
    {
        private readonly ICurrencyService _currencyService;

        public AttributeController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageGoodsTypeKey })]
        public ActionResult List(Guid typeId)
        {
            ViewBag.TypeId = typeId;
            return View();
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageGoodsTypeKey })]
        public ActionResult ListOnPage(Guid typeId)
        {
            var result = new DataTableJsonResult();

            //取参数值
            int draw, pageIndex, pageSize, totalCount;
            string sortColumn;
            bool isDesc;
            Request.GetDatatableParameters(out draw, out pageIndex, out pageSize, out sortColumn, out isDesc);
            result.draw = draw;

            //分页查询
            var list = _currencyService.GetListPaged<GoodsAttribute>(pageIndex, pageSize, a => a.GoodsTypeId.Equals(typeId), out totalCount, new OrderModelField { PropertyName = sortColumn, IsDesc = isDesc });

            result.data = list;
            result.recordsTotal = totalCount;
            result.recordsFiltered = totalCount;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageGoodsTypeKey })]
        public ActionResult Edit(Guid typeId, Guid? id = null)
        {
            ViewBag.GoodsTypes = _currencyService.GetAll<GoodsType>();

            GoodsAttribute attribute = null;
            if (id != null && id != Guid.Empty)
            {
                attribute = _currencyService.GetSingleById<GoodsAttribute>(id);
                if (attribute != null) { }
            }

            if (attribute == null)
            {
                attribute = new GoodsAttribute
                {
                    Id = KeyGenerator.GetGuidKey(),
                    GoodsTypeId = typeId,
                    InputType = Models.InputType.Select,
                    AttributeType = AttributeType.Single
                };
            }
            return View(attribute);
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageGoodsTypeKey })]
        public ActionResult EditOnPost(GoodsAttribute postAttribute)
        {
            var count = _currencyService.Count<GoodsAttribute>(ga => ga.GoodsTypeId.Equals(postAttribute.GoodsTypeId));
            if (count >= 2)
            {
                return Json(new DataJsonResult() { ErrorMessage = "每个类型最多添加两个属性" });
            }

            var attribute = _currencyService.GetSingleById<GoodsAttribute>(postAttribute.Id);
            if (count == 2 && attribute == null)
            {
                return Json(new DataJsonResult() { ErrorMessage = "每个类型最多添加两个属性" });
            }

            if (postAttribute.InputType == Models.InputType.Manual)
                postAttribute.Values = string.Empty;
            postAttribute.Values = postAttribute.Values.Trim().Replace(" ", "");
            if (attribute == null)
            {
                _currencyService.Create(postAttribute);
            }
            else
            {
                _currencyService.Update(postAttribute);
            }
            return Json(new DataJsonResult());
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageGoodsTypeKey })]
        public ActionResult Delete(Guid id)
        {
            var result = new DataJsonResult();
            _currencyService.DeleteByConditon<GoodsAttribute>(t => t.Id.Equals(id));
            return Json(result);
        }
    }
}