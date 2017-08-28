using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.Logging;
using BntWeb.Mall.Models;
using BntWeb.Security;
using LinqKit;

namespace BntWeb.Mall.Services
{
    public class GoodsCategoryService : IGoodsCategoryService
    {
        #region private定义
        private readonly ICurrencyService _currencyService;

        public ILogger Logger { get; set; }
        #endregion

        #region 构造
        public GoodsCategoryService(ICurrencyService currencyService)
        {
            _currencyService = currencyService;

            Logger = NullLogger.Instance;
        }
        #endregion

        #region 商品分类

        public List<GoodsCategory> GetShowIndexCategories()
        {
            using (var dbContext = new MallDbContext())
            {
                return dbContext.GoodsCategories.Where(c => c.ShowIndex).OrderByDescending(c => c.Sort).ToList();
            }
        }

        /// <summary>
        /// 获取所有类型
        /// </summary>
        /// <returns></returns>
        public List<GoodsCategory> GetCategories()
        {
            using (var dbContext = new MallDbContext())
            {
                return dbContext.GoodsCategories.OrderByDescending(me => me.Sort).ToList();
            }
        }

        public List<GoodsCategory> LoadByPage(out int totalCount, int pageIndex = 1, int pageSize = 10)
        {
            Expression<Func<GoodsCategory, bool>> expression = c => (c.ParentId == null || c.ParentId == Guid.Empty);
            List<GoodsCategory> list;
            using (var dbContext = new MallDbContext())
            {
                totalCount = dbContext.GoodsCategories.AsExpandable().Where(expression).Count();

                list = dbContext.GoodsCategories.AsExpandable().Where(expression).OrderByDescending(c => c.Sort).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }

            foreach (var item in list)
            {
                item.ChildCategories = GetAllChilds(item.Id);
            }

            return list;
        }

        private List<GoodsCategory> GetAllChilds(Guid parentId)
        {
            var list = _currencyService.GetList<GoodsCategory>(c => c.ParentId == parentId);
            foreach (var item in list)
            {
                item.ChildCategories = GetAllChilds(item.Id);
            }
            return list;
        }

        public GoodsCategory GetCategoryById(Guid id)
        {
            return _currencyService.GetSingleById<GoodsCategory>(id);
        }

        public GoodsCategory GetCategoryByName(string name)
        {
            return _currencyService.GetSingleByConditon<GoodsCategory>(me => me.Name == name);
        }
        /// <summary>
        /// 保存类型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Guid SaveCategory(GoodsCategory model)
        {
            bool result;
            string operateType = "";
            if (model.Id == Guid.Empty)
            {
                operateType = "创建";
                model.Id = KeyGenerator.GetGuidKey();
                result = _currencyService.Create(model);
            }
            else
            {
                operateType = "更新";
                result = _currencyService.Update(model);
            }
            if (!result)
                return Guid.Empty;
            Logger.Operation($"{operateType}商品分类-{model.Name}:{model.Id}", MallModule.Instance, SecurityLevel.Normal);
            return model.Id;
        }

        public Guid CreateCategory(GoodsCategory model)
        {
            bool result;
            using (var dbContext = new MallDbContext())
            {
                model.Id = KeyGenerator.GetGuidKey();
                if (model.ParentId == Guid.Empty)
                {
                    model.MergerId = model.Id.ToString();
                    model.Level = 1;
                }
                else
                {
                    var parentCategory = _currencyService.GetSingleById<GoodsCategory>(model.ParentId);
                    model.MergerId = parentCategory.MergerId + "," + model.Id;
                    model.Level = parentCategory.Level + 1;
                }
                dbContext.GoodsCategories.Add(model);

                result = dbContext.SaveChanges() > 0;
            }

            if (!result)
                return Guid.Empty;
            Logger.Operation($"创建商品分类-{model.Name}:{model.Id}", MallModule.Instance, SecurityLevel.Normal);
            return model.Id;
        }

        public Guid UpdateCategory(GoodsCategory model)
        {
            bool result;
            using (var dbContext = new MallDbContext())
            {
                if (model.ParentId == Guid.Empty)
                {
                    model.MergerId = model.Id.ToString();
                    model.Level = 1;
                }
                else
                {
                    var parentCategory = _currencyService.GetSingleById<GoodsCategory>(model.ParentId);
                    model.MergerId = parentCategory.MergerId + "," + model.Id;
                    model.Level = parentCategory.Level + 1;
                }
                var oldCategory = _currencyService.GetSingleById<GoodsCategory>(model.Id);
                bool parentChange = oldCategory.ParentId != model.ParentId;
                bool levelChange = oldCategory.Level != model.Level;

                dbContext.Entry(model).State = EntityState.Modified;

                //父级变更
                if (parentChange)
                {
                    var idStr = model.Id.ToString();
                    var childCategories = dbContext.GoodsCategories.Where(x => x.MergerId.Contains(idStr) && x.Id != model.Id);
                    foreach (var child in childCategories)
                    {
                        var mergerArr = child.MergerId.Split(new string[] { idStr }, StringSplitOptions.None);

                        child.MergerId = model.MergerId + mergerArr[1];

                        //等级发生变更
                        if (levelChange)
                            child.Level = child.MergerId.Split(',').Length;

                        dbContext.Entry(child).State = EntityState.Modified;
                    }
                }

                result = dbContext.SaveChanges() > 0;
            }


            if (!result)
                return Guid.Empty;
            Logger.Operation($"更新商品分类-{model.Name}:{model.Id}", MallModule.Instance, SecurityLevel.Normal);
            return model.Id;
        }


        /// <summary>
        /// 删除商品分类
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool DeleteCategory(GoodsCategory model)
        {
            var result = _currencyService.DeleteByConditon<GoodsCategory>(
                me => me.Id == model.Id);
            if (result > 0)
                Logger.Operation($"删除商品分类-{model.Name}:{model.Id}", MallModule.Instance, SecurityLevel.Warning);

            return result > 0;
        }

        /// <summary>
        /// 分类下商品数量
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public int HasGoodsCount(Guid categoryId)
        {
            using (var dbContext = new MallDbContext())
            {
                return dbContext.Goods.Count(me => me.CategoryId == categoryId);
            }
        }
        #endregion

        #region 分类关联
        public bool InsetCategoryRelation(GoodsCategoryRelation model)
        {
            model.Id = KeyGenerator.GetGuidKey();
            return _currencyService.Create(model);
        }

        public bool DeleteRelations(Guid goodsId)
        {
            return _currencyService.DeleteByConditon<GoodsCategoryRelation>(me => me.GoodsId == goodsId) > 0;
        }

        public List<GoodsCategoryRelation> GetCategoryRelations(Guid goodsId)
        {
            using (var dbContext = new MallDbContext())
            {
                return dbContext.GoodsCategoryRelations.Include(m => m.GoodsCategory).Where(me => me.GoodsId == goodsId).ToList();
            }
        }
        #endregion
    }
}
