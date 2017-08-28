using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using BntWeb.Core.SystemSettings.Models;
using BntWeb.Core.SystemSettings.Services;
using BntWeb.Core.SystemSettings.ViewModels;
using BntWeb.Data.Services;
using BntWeb.Mvc;
using BntWeb.Security;
using BntWeb.Security.Identity;
using BntWeb.Utility;
using BntWeb.Validation;
using BntWeb.Web.Extensions;

namespace BntWeb.Core.SystemSettings.Controllers
{
    public class DistrictController : Controller
    {
        private readonly ICurrencyService _currencyService;
        private readonly IDistrictService _districtService;

        public DistrictController(ICurrencyService currencyService, IDistrictService districtService)
        {
            _currencyService = currencyService;
            _districtService = districtService;
        }


        [AdminAuthorize(PermissionsArray = new[] { Permissions.ViewDistrictKey })]
        public ActionResult Index()
        {

            return View();
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ViewDistrictKey })]
        public ActionResult LoadByParentId()
        {
            var parentId = Request.Get("pid", "-1");
            var districts =
                _currencyService.GetList<District>(d => d.ParentId.Equals(parentId, StringComparison.OrdinalIgnoreCase));
            return Json(districts);
        }

        public ActionResult LoadChilds(string parentId)
        {
            Argument.ThrowIfNullOrEmpty(parentId, "父级地区Id");

            var districts =
                _currencyService.GetList<District>(d => d.ParentId.Equals(parentId, StringComparison.OrdinalIgnoreCase));
            return Json(districts, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [AdminAuthorize(PermissionsArray = new[] { Permissions.EditDistrictKey })]
        public ActionResult EditOnPost(EditDistrictViewModel editDistrict)
        {
            var parent = _currencyService.GetSingleById<District>(editDistrict.ParentId);

            var district = new District
            {
                Id = editDistrict.Id,
                ParentId = parent.Id,
                FullName = editDistrict.FullName,
                ShortName = editDistrict.ShortName,
                Lng = editDistrict.Lng,
                Lat = editDistrict.Lat,
                Sort = editDistrict.Sort,
                Level = parent.Level + 1,
                Position = " tr_" + parent.Id,
                MergerName = (parent.MergerName + "," + editDistrict.FullName).Trim(','),
                MergerShortName = (parent.MergerShortName + "," + editDistrict.ShortName).Trim(',')
            };

            return Json(_districtService.Save(district, editDistrict.EditMode == 0));
        }

        [HttpPost]
        [AdminAuthorize(PermissionsArray = new[] { Permissions.EditDistrictKey })]
        public ActionResult Delete(string districtId)
        {
            return Json(_districtService.Delete(districtId));
        }

        public ActionResult DistrictSelectPartial(string controlId, string province, string city, string district, string street, DistrictSelectLevel level = DistrictSelectLevel.Street)
        {
            Argument.ThrowIfNullOrEmpty(controlId, "controlId");

            ViewBag.ControlId = controlId;
            ViewBag.ProvinceId = province;
            ViewBag.CityId = city;
            ViewBag.DistrictId = district;
            ViewBag.StreetId = street;
            ViewBag.Level = level;

            ViewBag.Provinces = _currencyService.GetList<District>(d => d.ParentId.Equals("0", StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(province) && level >= DistrictSelectLevel.City)
                ViewBag.Citys = _currencyService.GetList<District>(d => d.ParentId.Equals(province, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(city) && level >= DistrictSelectLevel.District)
                ViewBag.Districts = _currencyService.GetList<District>(d => d.ParentId.Equals(city, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(district) && level >= DistrictSelectLevel.Street)
                ViewBag.Streets = _currencyService.GetList<District>(d => d.ParentId.Equals(district, StringComparison.OrdinalIgnoreCase));

            return PartialView("_PartialDistrictSelect");
        }
    }
}