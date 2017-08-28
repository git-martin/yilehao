using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using BntWeb.Data.Services;
using BntWeb.Logistics.Models;
using BntWeb.Logistics.Services;
using BntWeb.Mvc;
using BntWeb.Security;
using BntWeb.Utility.Extensions;
using BntWeb.Web.Extensions;

namespace BntWeb.Logistics.Controllers
{
    public class ShippingController : Controller
    {
        private readonly IShippingService _shippingService;
        private readonly ICurrencyService _currencyService;
        private readonly IUserContainer _userContainer;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="shippingService"></param>
        /// <param name="currencyService"></param>
        /// <param name="userContainer"></param>
        public ShippingController(IShippingService shippingService,ICurrencyService currencyService, IUserContainer userContainer)
        {
            _shippingService = shippingService;
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



            Expression<Func<Models.Shipping, bool>> expression =
                l => ((checkName || l.Name.Equals(name)) &&
                     (int)l.Status > (int)ShippingStatus.Delete);

            Expression<Func<Models.Shipping, object>> orderByExpression;
            //设置排序
            switch (sortColumn)
            {
                default:
                    orderByExpression = act => new { act.CreateTime };
                    break;
            }

            //分页查询
            var list = _shippingService.GetListPaged(pageIndex, pageSize, expression, orderByExpression,
                isDesc, out totalCount);

            result.data = list;
            result.recordsTotal = totalCount;
            result.recordsFiltered = totalCount;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AdminAuthorize(PermissionsArray = new[] { Permissions.SwitchShippingKey })]
        public ActionResult Switch(Guid shippingId)
        {
            var result = new DataJsonResult();
            Models.Shipping shipping = _currencyService.GetSingleById<Models.Shipping>(shippingId);

            if (shipping != null&& shipping.Status != ShippingStatus.Delete)
            {
                string msg = "";
                if (shipping.Status == ShippingStatus.Disabled)
                {
                    shipping.Status = ShippingStatus.Enabled;
                    msg = "启用";
                }
                else if (shipping.Status == ShippingStatus.Enabled)
                {
                    shipping.Status = ShippingStatus.Disabled;
                    msg = "禁用";
                }
                var isDelete = _shippingService.UpdateShipping(shipping);

                if (!isDelete)
                {
                    result.ErrorMessage = msg+"失败!";
                }
            }
            else
            {
                result.ErrorMessage = "配送类型不存在！";
            }

            return Json(result);
        }

        [HttpPost]
        [AdminAuthorize]
        public ActionResult SetDefault(Guid shippingId)
        {
            var result = new DataJsonResult();
            Models.Shipping shipping = _currencyService.GetSingleById<Models.Shipping>(shippingId);
            if (shipping != null)
            {
                _shippingService.SetDefault(shippingId);
            }
            else
            {
                result.ErrorMessage = "配送类型不存在！";
            }

           

            return Json(result);
        }


    }
}