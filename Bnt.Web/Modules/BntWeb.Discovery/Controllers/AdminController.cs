using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using BntWeb.Discovery.Services;
using BntWeb.Discovery.ViewModels;
using BntWeb.Data.Services;
using BntWeb.FileSystems.Media;
using BntWeb.Mall.Models;
using BntWeb.Mvc;
using BntWeb.Security;
using BntWeb.Utility.Extensions;
using BntWeb.Web.Extensions;

namespace BntWeb.Discovery.Controllers
{
    public class AdminController : Controller
    {
        private readonly IDiscoveryService _discoveryService;
        private readonly IStorageFileService _storageFileService;
        private readonly ICurrencyService _currencyService;
        private readonly IUserContainer _userContainer;
        private const string DiscoveryImages = "DiscoveryImages";

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="discoveryService"></param>
        /// <param name="storageFileService"></param>
        /// <param name="currencyService"></param>
        /// <param name="userContainer"></param>
        public AdminController(IDiscoveryService discoveryService, IStorageFileService storageFileService, ICurrencyService currencyService, IUserContainer userContainer)
        {
            _discoveryService = discoveryService;
            _storageFileService = storageFileService;
            _currencyService = currencyService;
            _userContainer = userContainer;
        }


        [AdminAuthorize(PermissionsArray = new[] { Permissions.ViewDiscoveryKey })]
        public ActionResult List()
        {
            return View();
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ViewDiscoveryKey })]
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
            var status = Request.Get("extra_search[Status]");
            var checkStatus = string.IsNullOrWhiteSpace(status);

            var title = Request.Get("extra_search[Title]");
            var checkTitle = string.IsNullOrWhiteSpace(title);

            var createName = Request.Get("extra_search[CreateName]");
            var checkCreateName = string.IsNullOrWhiteSpace(createName);

            var createTimeBegin = Request.Get("extra_search[CreateTimeBegin]");
            var checkCreateTimeBegin = string.IsNullOrWhiteSpace(createTimeBegin);
            var createTimeBeginTime = createTimeBegin.To<DateTime>();

            var createTimeEnd = Request.Get("extra_search[CreateTimeEnd]");
            var checkCreateTimeEnd = string.IsNullOrWhiteSpace(createTimeEnd);
            var createTimeEndTime = createTimeEnd.To<DateTime>();

            Expression<Func<Models.Discovery, bool>> expression =
                l => (checkTitle || l.Title.Contains(title)) &&
                     (checkCreateName || l.CreateName.Contains(createName)) &&
                     (checkStatus || ((int)l.Status).ToString().Equals(status)) &&
                     (checkCreateTimeBegin || l.CreateTime >= createTimeBeginTime) &&
                     (checkCreateTimeEnd || l.CreateTime <= createTimeEndTime) &&
                     (l.Status > 0);

            //分页查询
            var list = _currencyService.GetListPaged<Models.Discovery>(pageIndex, pageSize, expression, out totalCount, new OrderModelField { PropertyName = sortColumn, IsDesc = isDesc });

            result.data = list;
            result.recordsTotal = totalCount;
            result.recordsFiltered = totalCount;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.EditDiscoveryKey })]
        public ActionResult Edit(Guid? id = null)
        {
            Models.Discovery model = new Models.Discovery();
            if (id != null && id != Guid.Empty)
                model = _currencyService.GetSingleById<Models.Discovery>(id);

            ViewBag.RelationGoods = _discoveryService.GetDiscoveryRelationGoods(model.Id);
            return View(model);
        }
        
        [ValidateInput(false)]
        [HttpPost]
        [AdminAuthorize(PermissionsArray = new[] { Permissions.EditDiscoveryKey })]
        public ActionResult EditOnPost(EditDiscoveryViewModel editDiscovery)
        {
            var result = new DataJsonResult();
            Models.Discovery model = new Models.Discovery();

            if (editDiscovery.Id != Guid.Empty)
                model = _currencyService.GetSingleById<Models.Discovery>(editDiscovery.Id);
            
            model.Title = editDiscovery.Title;
            model.Source = editDiscovery.Source;
            model.Content = editDiscovery.Content;
            model.Blurb = editDiscovery.Blurb;
            model.Author = editDiscovery.Author;
            
            model.Id=_discoveryService.SaveDiscovery(model, editDiscovery.Goods);

            if (model.Id != Guid.Empty)
            {
                //添加图片关联关系
                _storageFileService.ReplaceFile(model.Id, DiscoveryModule.Key, DiscoveryModule.DisplayName, editDiscovery.DiscoveryImages, DiscoveryImages);
            }
            else
            {
                result.ErrorMessage = "保存失败";
            }

            return Json(result);
        }


        [AdminAuthorize(PermissionsArray = new[] { Permissions.EditDiscoveryKey })]
        public ActionResult GetGoods(string keyword)
        {
            var result = new DataJsonResult();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                int totalCount;
                Expression<Func<Goods, bool>> expression =
                    l => l.Name.ToString().Contains(keyword) && l.Status == GoodsStatus.InSale;
                var goodsList = _currencyService.GetListPaged<Goods>(1, 10, expression, out totalCount, new OrderModelField { PropertyName = "CreateTime", IsDesc = true });

                result.Data = goodsList;
            }
            return Json(result);
        }


        [HttpPost]
        [AdminAuthorize(PermissionsArray = new[] { Permissions.DeleteDiscoveryKey })]
        public ActionResult Delete(Guid id)
        {
            var result = new DataJsonResult();
            _discoveryService.DeleteDiscovery(id);
            return Json(result);
        }
    }
}