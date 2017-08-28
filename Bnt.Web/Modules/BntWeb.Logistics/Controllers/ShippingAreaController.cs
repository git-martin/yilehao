using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using BntWeb.Core.SystemSettings.Models;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.Logistics.Models;
using BntWeb.Logistics.Services;
using BntWeb.Logistics.ViewModels;
using BntWeb.Mvc;
using BntWeb.Security;
using BntWeb.Web.Extensions;

namespace BntWeb.Logistics.Controllers
{
    public class ShippingAreaController : Controller
    {
        private readonly IShippingAreaService _shippingAreaService;
        private readonly ICurrencyService _currencyService;
        private readonly IUserContainer _userContainer;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="shippingAreaService"></param>
        /// <param name="currencyService"></param>
        /// <param name="userContainer"></param>
        public ShippingAreaController(IShippingAreaService shippingAreaService, ICurrencyService currencyService, IUserContainer userContainer)
        {
            _shippingAreaService = shippingAreaService;
            _currencyService = currencyService;
            _userContainer = userContainer;
        }

        [AdminAuthorize]
        public ActionResult List()
        {
            return View();
        }

        [AdminAuthorize]
        public ActionResult ListOnPage()
        {
            //return Json("", JsonRequestBehavior.AllowGet);
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

            Expression<Func<Models.ShippingArea, bool>> expression =
                l => ((checkName || l.Name.Equals(name)) &&
                     (int)l.Status > (int)ShippingAreaStatus.Delete);

            Expression<Func<Models.ShippingArea, object>> orderByExpression;
            //设置排序
            switch (sortColumn)
            {
                default:
                    orderByExpression = act => new { act.CreateTime };
                    break;
            }

            //分页查询
            var list = _shippingAreaService.GetListPaged(pageIndex, pageSize, expression, orderByExpression,
                isDesc, out totalCount);

            result.data = list;
            result.recordsTotal = totalCount;
            result.recordsFiltered = totalCount;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.EditShippingAreaKey })]
        public ActionResult Edit(Guid? id = null)
        {

            ShippingArea shippingArea = null;
            var shippingCodeList = new string[] { };
            if (id != null && id != Guid.Empty)
            {
                shippingArea = _shippingAreaService.GetById(id.Value);
                ViewBag.IsNew = false;
                List<ShippingAreaFee> feeList = _shippingAreaService.GetFeeListByShippingAreaId(id.Value);
                ViewBag.FeeList = feeList;
                ViewBag.CodeList = feeList.Select(x => x.AreaId).ToArray();
            }

            if (shippingArea == null)
            {
                shippingArea = new ShippingArea
                {
                    Id = KeyGenerator.GetGuidKey(),
                    IsDefualt = DefaultStatus.Normal

                };
                ViewBag.IsNew = true;
            }
            var drovinceList = _currencyService.GetList<District>(
                   d => d.ParentId.Equals("0", StringComparison.OrdinalIgnoreCase)).Select(x => new DistrictWithChilViewModel
                   {
                       Id = x.Id,
                       ParentId = x.ParentId,
                       ShortName = x.ShortName,
                       Sort = x.Sort,
                       DistrictChil = _currencyService.GetList<District>(
                   d => d.ParentId.Equals(x.Id, StringComparison.OrdinalIgnoreCase)).Select(y => new DistrictViewModel
                   {
                       Id = y.Id,
                       ParentId = y.ParentId,
                       ShortName = y.ShortName,
                       Sort = y.Sort
                   }).ToList()
                   }).ToList();
            ViewBag.DrovinceList = drovinceList;


            return View(shippingArea);
        }

        [HttpPost]
        [AdminAuthorize(PermissionsArray = new[] { Permissions.EditShippingAreaKey })]
        [System.Web.Mvc.ValidateInput(false)]
        public ActionResult EditOnPost(ShippingAreaViewModel postModel)
        {
            var result = new DataJsonResult();
            var shippingArea = _currencyService.GetSingleById<ShippingArea>(postModel.Id);
            var isNew = false;

            if (postModel.AreaId == null || postModel.AreaId.Count == 0)
            {
                result.ErrorMessage = "请选择区域";
            }
            else
            {
                var areaNames = "";
                List<ShippingAreaFee> feeList = new List<ShippingAreaFee>();
                foreach (var areaId in postModel.AreaId)
                {
                    ShippingAreaFee feeInfo = new ShippingAreaFee();
                    feeInfo.Id = KeyGenerator.GetGuidKey();
                    feeInfo.ShippingAreaId = postModel.Id;
                    feeInfo.AreaId = areaId;
                    feeInfo.AreaName = _currencyService.GetSingleById<District>(areaId).FullName;
                    feeInfo.Freight = postModel.Freight;
                    feeInfo.Weight = postModel.Weight;
                    feeInfo.SFreight = postModel.SFreight;
                    feeInfo.IsDefualt = postModel.IsDefualt;
                    feeList.Add(feeInfo);
                    areaNames += feeInfo.AreaName + ",";
                }

                areaNames = areaNames.Substring(0, areaNames.Length - 1);
                if (shippingArea == null)
                {
                    shippingArea = new ShippingArea
                    {
                        Id = postModel.Id,
                        Name = postModel.Name,
                        AreaNames = areaNames,
                        Freight = postModel.Freight,
                        Sort = postModel.Sort,
                        Weight = postModel.Weight,
                        SFreight = postModel.SFreight,
                        IsDefualt = postModel.IsDefualt
                    };
                    isNew = true;
                }
                else
                {
                    shippingArea.Name = postModel.Name;
                    shippingArea.AreaNames = areaNames;
                    shippingArea.Freight = postModel.Freight;
                    shippingArea.Sort = postModel.Sort;
                    shippingArea.Weight = postModel.Weight;
                    shippingArea.SFreight = postModel.SFreight;
                    shippingArea.IsDefualt = postModel.IsDefualt;
                }

                result.Success = _shippingAreaService.SaveInfo(shippingArea, feeList, isNew);
            }

            return Json(result);
        }



        [AdminAuthorize(PermissionsArray = new[] { Permissions.DeleteShippingAreaKey })]
        public ActionResult Delete(Guid id)
        {
            var result = new DataJsonResult();
            if (!_shippingAreaService.Delete(id))
            {
                result.ErrorMessage = "未知异常，删除失败";
            }

            return Json(result);
        }

        [AdminAuthorize]
        public ActionResult LoadDistrictByParentId(string parentId)
        {
            StringBuilder htmlStr = new StringBuilder();
            //var parentId = Request.Get("pid", "-1");
            var districtList =
                _currencyService.GetList<District>(d => d.ParentId.Equals(parentId, StringComparison.OrdinalIgnoreCase));
            if (districtList != null)
            {
                foreach (var info in districtList)
                {
                    htmlStr.AppendFormat("<option value=\"{0}\">{1}</option>", info.Id, info.ShortName);
                }
            }

            return Json(htmlStr.ToString(), JsonRequestBehavior.AllowGet);
        }
    }
}