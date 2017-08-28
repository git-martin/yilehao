/* 
    ======================================================================== 
        File name：		GoodsCategorieController
        Module:			
        Author：		Daniel.Wu（wujb）
        Create Time：		2016/7/4 11:44:48
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BntWeb.Data.Services;
using BntWeb.FileSystems.Media;
using BntWeb.Mall.Models;
using BntWeb.Mall.Services;
using BntWeb.Mall.ViewModels;
using BntWeb.Mvc;
using BntWeb.Security;
using BntWeb.Utility.Extensions;
using BntWeb.Validation;
using BntWeb.Web.Extensions;

namespace BntWeb.Mall.Controllers
{
    public class GoodsCategoryController : Controller
    {
        private readonly ICurrencyService _currencyService;
        private readonly IGoodsService _goodsService;
        private readonly IGoodsCategoryService _goodsCategoryService;
        private readonly IStorageFileService _storageFileService;
        private const string CategoryImage = "CategoryImage";

        public GoodsCategoryController(ICurrencyService currencyService, IGoodsService goodsService, IGoodsCategoryService goodsCategoryService, IStorageFileService storageFileService)
        {
            _currencyService = currencyService;
            _goodsService = goodsService;
            _goodsCategoryService = goodsCategoryService;
            _storageFileService = storageFileService;
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageGoodsCategorieKey })]
        public ActionResult List()
        {

            return View();
        }


        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageGoodsCategorieKey })]
        public ActionResult ListOnPage()
        {
            var result = new DataJsonResult();
            var pageIndex = Request.Get("pageIndex").To(1);
            var pageSize = Request.Get("pageSize").To(10);
            int totalCount;
            var goodsCategories = _goodsCategoryService.LoadByPage(out totalCount, pageIndex, pageSize);
            result.Data = new { TotalCount = totalCount, Categories = goodsCategories };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageGoodsCategorieKey })]
        public ActionResult Create(Guid? parentId)
        {
            //商品分类
            var categories = _goodsService.GetCategories();
            if (categories == null || categories.Count == 0)
            {
                categories = new List<GoodsCategory>();
            }
            categories.Add(new GoodsCategory { Id = Guid.Empty, ParentId = Guid.Empty, Name = "顶级" });
            ViewBag.CategoriesJson = categories.Select(me => new { id = me.Id, pId = me.ParentId, name = me.Name }).ToList().ToJson();
            ViewBag.GoodsType = _currencyService.GetList<GoodsType>(t => t.Enabled);


            GoodsCategory parentCategory = null;
            if (parentId != null)
                parentCategory = _currencyService.GetSingleById<GoodsCategory>(parentId);
            ViewBag.ParentCategory = parentCategory;

            return View("Edit", new GoodsCategory());
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageGoodsCategorieKey })]
        public ActionResult Edit(Guid categoryId)
        {
            //商品分类
            var categories = _goodsService.GetCategories();
            if (categories == null || categories.Count == 0)
                throw new BntWebCoreException("商品分类异常");

            categories.Add(new GoodsCategory { Id = Guid.Empty, ParentId = Guid.Empty, Name = "顶级" });
            RemoveChild(categories, categoryId);
            ViewBag.CategoriesJson = categories.Select(me => new { id = me.Id, pId = me.ParentId, name = me.Name }).ToList().ToJson();
            ViewBag.GoodsType = _currencyService.GetList<GoodsType>(t => t.Enabled);


            var category = _currencyService.GetSingleById<GoodsCategory>(categoryId);

            GoodsCategory parentCategory = null;
            if (category != null && category.ParentId != Guid.Empty)
                parentCategory = _currencyService.GetSingleById<GoodsCategory>(category.ParentId);
            ViewBag.ParentCategory = parentCategory;


            return View(category);
        }

        private void RemoveChild(List<GoodsCategory> categories, Guid categoryId)
        {
            var childs = categories.Where(c => c.ParentId == categoryId).ToArray();
            foreach (var child in childs)
            {
                RemoveChild(categories, child.Id);
                categories.Remove(child);
            }
            categories.Remove(categories.FirstOrDefault(c => c.Id == categoryId));
        }

        /// <summary>
        /// 更新类别
        /// </summary>
        /// <param name="editModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageGoodsCategorieKey })]
        public ActionResult EditOnPost(GoodsCategoryViewModel editModel)
        {
            var model = new GoodsCategory
            {
                Id = editModel.Id,
                ParentId = editModel.ParentId,
                Descirption = editModel.Descirption,
                Name = editModel.Name,
                Sort = editModel.Sort,
                ShowIndex = editModel.ShowIndex,
                GoodsTypeId = editModel.GoodsTypeId
            };

            var result = new DataJsonResult();

            if (editModel.Id == Guid.Empty)
                _goodsCategoryService.CreateCategory(model);
            else
            {
                _goodsCategoryService.UpdateCategory(model);
            }

            if (model.Id == Guid.Empty)
            {
                result.ErrorMessage = "输入数据错误";
                result.Success = false;
            }
            else
            {
                //处理主图图片关联
                _storageFileService.ReplaceFile(model.Id, MallModule.Key, MallModule.DisplayName, editModel.CategoryImage, CategoryImage);

                result.Data = model.Id;
            }

            return Json(result);
        }


        [HttpPost]
        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageGoodsCategorieKey })]
        public ActionResult Delete(Guid categoryId)
        {
            var result = new DataJsonResult();
            var model = _currencyService.GetSingleById<GoodsCategory>(categoryId);

            if (model != null)
            {

                var childs = _currencyService.GetList<GoodsCategory>(me => me.ParentId == model.Id);
                if (childs != null && childs.Count > 0)
                {
                    result.Success = false;
                    result.ErrorMessage = $"[{model.Name}] 不是末级分类，不能直接删除!";
                }
                else
                {

                    var goodsCount = _goodsCategoryService.HasGoodsCount(categoryId);
                    if (goodsCount > 0)
                    {
                        result.Success = false;
                        result.ErrorMessage = $"[{model.Name}] 分类下有商品，不能直接删除!";
                    }
                    else
                    {
                        var isDelete = _goodsCategoryService.DeleteCategory(model);

                        if (!isDelete)
                        {
                            result.Success = false;
                            result.ErrorMessage = "删除失败!";
                        }
                    }
                }
            }
            else
            {
                result.Success = false;
                result.ErrorMessage = "类别不存在！";
            }

            return Json(result);
        }
    }
}