using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using BntWeb.FileSystems.Media;
using BntWeb.Mall.ApiModels;
using BntWeb.Mall.Services;
using BntWeb.WebApi.Models;

namespace BntWeb.Mall.ApiControllers
{
    public class GoodsCategoryController : BaseApiController
    {
        private readonly IGoodsCategoryService _goodsCategoryServices;
        private readonly IStorageFileService _storageFileService;

        public GoodsCategoryController(IGoodsCategoryService goodsCategoryServices, IStorageFileService storageFileService)
        {
            _goodsCategoryServices = goodsCategoryServices;
            _storageFileService = storageFileService;
        }

        /// <summary>
        /// 递归商品分类树结构
        /// </summary>
        /// <param name="toType"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        private List<GoodsCategoryModel> setTypeTree(GoodsCategoryModel toType, List<Models.GoodsCategory> types)
        {
            var childTypes = types.Where(me => me.ParentId == toType.Id).OrderByDescending(me => me.Sort);
            toType.ChildGoodsCategorys = new List<GoodsCategoryModel>();
            foreach (var child in childTypes)
            {
                var item = new GoodsCategoryModel();
                item.Id = child.Id;
                item.Name = child.Name;
                item.ChildGoodsCategorys = setTypeTree(item, types);
                var mainImage = _storageFileService.GetFiles(child.Id, MallModule.Key, "CategoryImage").FirstOrDefault();
                item.CategoryImage = mainImage?.Simplified();
                toType.ChildGoodsCategorys.Add(item);
            }
            return toType.ChildGoodsCategorys;
        }

        /// <summary>
        /// 获取商家分类树结构
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResult GetGoodsCategory()
        {
            var list = _goodsCategoryServices.GetCategories();

            List<GoodsCategoryModel> typeList = new List<GoodsCategoryModel>();

            foreach (var item in list.Where(me => me.ParentId == Guid.Empty).OrderByDescending(me => me.Sort))
            {
                var type = new GoodsCategoryModel();
                type.Id = item.Id;
                type.Name = item.Name;
                type.ChildGoodsCategorys = setTypeTree(type, list);
                var mainImage = _storageFileService.GetFiles(item.Id, MallModule.Key, "CategoryImage").FirstOrDefault();
                type.CategoryImage = mainImage?.Simplified();
                typeList.Add(type);
            }

            var result = new ApiResult();
            var data = new
            {
                GoodsCategorys = typeList
            };

            result.SetData(data);

            return result;
        }
        
    }
}
