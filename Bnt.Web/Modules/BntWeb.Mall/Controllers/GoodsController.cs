using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using BntWeb.Config.Models;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.FileSystems.Media;
using BntWeb.Logging;
using BntWeb.Mall.Models;
using BntWeb.Mall.Services;
using BntWeb.Mall.ViewModels;
using BntWeb.Mvc;
using BntWeb.Security;
using BntWeb.Services;
using BntWeb.Utility.Extensions;
using BntWeb.Web.Extensions;

namespace BntWeb.Mall.Controllers
{
    public class GoodsController : Controller
    {
        private readonly ICurrencyService _currencyService;
        private readonly IGoodsService _goodsService;
        private readonly IGoodsCategoryService _goodsCategoryService;
        private readonly IStorageFileService _storageFileService;
        private readonly IConfigService _configService;
        private const string MainImage = "MainImage";

        public GoodsController(ICurrencyService currencyService, IGoodsService goodsService, IGoodsCategoryService goodsCategoryService, IStorageFileService storageFileService, IConfigService configService)
        {
            _currencyService = currencyService;
            _goodsService = goodsService;
            _goodsCategoryService = goodsCategoryService;
            _storageFileService = storageFileService;
            _configService = configService;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageGoodsKey })]
        public ActionResult List()
        {
            var categories = _goodsService.GetCategories();
            //if (categories == null || categories.Count == 0)
            //    throw new BntWebCoreException("商品分类异常");
            ViewBag.CategoriesJson = categories?.Select(me => new { id = me.Id, pId = me.ParentId, name = me.Name }).ToList().ToJson();
            return View();
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageGoodsKey })]
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
            var name = Request.Get("extra_search[Name]");
            var checkName = string.IsNullOrWhiteSpace(name);

            var goodsNo = Request.Get("extra_search[GoodsNo]");
            var checkGoodsNo = string.IsNullOrWhiteSpace(goodsNo);

            var status = Request.Get("extra_search[Status]");
            var checkStatus = string.IsNullOrWhiteSpace(status);

            var categoryId = Request.Get("extra_search[CategoryId]");
            var checkCategoryId = string.IsNullOrWhiteSpace(categoryId);

            Expression<Func<Models.GoodsCategoryNameView, bool>> expression =
                l => (checkName || l.Name.ToString().Contains(name)) &&
                     (checkGoodsNo || l.GoodsNo.Contains(goodsNo)) &&
                     (checkStatus || ((int)l.Status).ToString().Equals(status)) &&
                     (checkCategoryId || l.MergerId.Contains(categoryId)) && l.Status != GoodsStatus.Delete;

            //分页查询
            var list = _currencyService.GetListPaged<GoodsCategoryNameView>(pageIndex, pageSize, expression, out totalCount, new OrderModelField { PropertyName = sortColumn, IsDesc = isDesc }).Select(g => new ListGoodsCategoryViewModel(g));

            result.data = list;
            result.recordsTotal = totalCount;
            result.recordsFiltered = totalCount;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageGoodsKey })]
        public ActionResult Edit(Guid? id = null)
        {
            //商品分类
            var categories = _goodsService.GetCategories();
            if (categories == null || categories.Count == 0)
                throw new BntWebCoreException("商品分类异常");
            ViewBag.CategoriesJson = categories.Select(me => new { id = me.Id, pId = me.ParentId, name = me.Name }).ToList().ToJson();
            ViewBag.GoodsType = _currencyService.GetList<GoodsType>(t => t.Enabled);
            ViewBag.GoodsBrand = _currencyService.GetAll<GoodsBrand>().OrderByDescending(b => b.Sort).ToList();
            var systemConfig = _configService.Get<SystemConfig>();
            ViewBag.MaxLevel = systemConfig.MaxLevel;

            Goods goods = null;
            var selectedAttrs = new List<SelectedAttrViewModel>();
            if (id != null && id != Guid.Empty)
            {
                goods = _goodsService.LoadFullGoods(id.Value);
                if (goods != null)
                {
                    //获取扩展属性
                    var categoryRelations = _goodsCategoryService.GetCategoryRelations(goods.Id);
                    var categoryIds = string.Join(",", categoryRelations.Select(me => me.CategoryId).ToList());
                    var categoryNames = string.Join(",", categoryRelations.Select(me => me.GoodsCategory.Name).ToList());
                    ViewBag.ExtendCategoryIds = categoryIds;
                    ViewBag.ExtendCategoryNames = categoryNames;
                    ViewBag.Commissions = _currencyService.GetList<GoodsCommission>(c => c.GoodsId.Equals(goods.Id),
                        new OrderModelField { PropertyName = "Level", IsDesc = false });

                    //拼接已经选中的属性数据
                    foreach (var singleGoods in goods.SingleGoods)
                    {
                        foreach (var attribute in singleGoods.Attributes)
                        {
                            var attr = selectedAttrs.FirstOrDefault(a => a.Id.Equals(attribute.AttributeId));
                            if (attr == null)
                            {
                                selectedAttrs.Add(new SelectedAttrViewModel
                                {
                                    Id = attribute.AttributeId,
                                    Vals = new List<string> { attribute.AttributeValue }
                                });
                            }
                            else
                            {
                                if (!attr.Vals.Contains(attribute.AttributeValue))
                                {
                                    attr.Vals.Add(attribute.AttributeValue);
                                }
                            }
                        }
                    }
                }
            }

            if (goods == null)
            {
                goods = new Goods
                {
                    Id = KeyGenerator.GetGuidKey()
                };
            }

            ViewBag.CategoryName = categories.Find(me => me.Id == goods.CategoryId) != null
                ? categories.Find(me => me.Id == goods.CategoryId).Name
                : "";
            ViewBag.CurrentSingleGoods = goods.SingleGoods.ToJson();
            ViewBag.CurrentAttrs = selectedAttrs.ToJson();
            if (goods.IsGroupon)
            {
                ViewBag.ToGrouponSeconds = goods.GrouponStartTime > DateTime.Now
                    ? (goods.GrouponStartTime - DateTime.Now).TotalSeconds
                    : 0;
                ViewBag.GrouponingSeconds = (goods.GrouponEndTime - DateTime.Now).TotalSeconds;
            }
            return View(goods);
        }

        [HttpPost]
        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageGoodsKey })]
        [System.Web.Mvc.ValidateInput(false)]
        public ActionResult EditOnPost(GoodsViewModel postGoods)
        {
            var result = new DataJsonResult();
            var category = _currencyService.GetSingleById<GoodsCategory>(postGoods.CategoryId);
            if (category.ParentId == Guid.Empty)
            {
                result.ErrorMessage = "一级分类下不能添加产品";
            }
            else
            {
                postGoods.SingleGoods = postGoods.SingleGoodsJson.DeserializeJsonToList<SingleGoodsViewModel>();
                var goods = _currencyService.GetSingleById<Goods>(postGoods.Id);
                var isNew = false;
                if (goods == null)
                {
                    goods = new Goods
                    {
                        Id = postGoods.Id,
                        CategoryId = postGoods.CategoryId,
                        BrandId = postGoods.BrandId,
                        Name = postGoods.Name,
                        OriginalPrice = postGoods.OriginalPrice,
                        GoodsType = postGoods.GoodsType,
                        GoodsNo = postGoods.GoodsNo,
                        Description = postGoods.Description,
                        Status = GoodsStatus.NotInSale,
                        CreateTime = DateTime.Now,
                        SingleGoods = new List<SingleGoods>(),
                        FreeShipping = postGoods.FreeShipping,
                        IsHot = postGoods.IsHot,
                        IsNew = postGoods.IsNew,
                        IsBest = postGoods.IsBest,
                        UsualWeight = postGoods.UsualWeight,
                        IsPresell = postGoods.IsPresell,
                        Brief = postGoods.Brief,
                        Sort = postGoods.Sort
                    };
                    goods.LastUpdateTime = goods.CreateTime;
                    isNew = true;
                }
                else
                {
                    goods.CategoryId = postGoods.CategoryId;
                    goods.BrandId = postGoods.BrandId;
                    goods.Name = postGoods.Name;
                    goods.OriginalPrice = postGoods.OriginalPrice;
                    goods.GoodsNo = postGoods.GoodsNo;
                    goods.GoodsType = postGoods.GoodsType;
                    goods.Description = postGoods.Description;
                    goods.SingleGoods = new List<SingleGoods>();
                    goods.LastUpdateTime = DateTime.Now;
                    goods.FreeShipping = postGoods.FreeShipping;
                    goods.IsHot = postGoods.IsHot;
                    goods.IsNew = postGoods.IsNew;
                    goods.IsBest = postGoods.IsBest;
                    goods.UsualWeight = postGoods.UsualWeight;
                    goods.IsPresell = postGoods.IsPresell;
                    goods.Brief = postGoods.Brief;
                    goods.Sort = postGoods.Sort;
                }

                foreach (var postSingleGoods in postGoods.SingleGoods)
                {
                    if (postSingleGoods.Stock > 0)
                    {
                        var singleGoods = new SingleGoods
                        {
                            Id = postSingleGoods.Id,
                            CreateTime = goods.CreateTime,
                            Stock = postSingleGoods.Stock,
                            Unit = postSingleGoods.Unit,
                            Price = postSingleGoods.Price,
                            GoodsId = goods.Id,
                            Weight = postSingleGoods.Weight,
                            GrouponPrice = postSingleGoods.GrouponPrice,
                            Attributes = new List<SingleGoodsAttribute>(),
                        };

                        foreach (var attr in postSingleGoods.Attrs)
                        {
                            singleGoods.Attributes.Add(new SingleGoodsAttribute
                            {
                                Id = KeyGenerator.GetGuidKey(),
                                AttributeId = attr.Id,
                                AttributeValue = attr.Val.Trim(),
                                SingleGoodsId = singleGoods.Id
                            });
                        }
                        singleGoods.Image = new StorageFile { Id = postSingleGoods.Image?.Id ?? Guid.Empty }.Simplified();
                        goods.SingleGoods.Add(singleGoods);
                    }
                }

                goods.ShopPrice = goods.SingleGoods.Min(g => g.Price);
                goods.Stock = goods.SingleGoods.Sum(g => g.Stock);
                goods.GrouponPrice = goods.SingleGoods.Min(g => g.GrouponPrice);

                result.Success = _goodsService.SaveGoods(goods, isNew);

                if (result.Success)
                {
                    //扩展分类
                    _goodsCategoryService.DeleteRelations(goods.Id);
                    if (!string.IsNullOrWhiteSpace(postGoods.ExtendCategoryIds))
                        foreach (var cId in postGoods.ExtendCategoryIds.Split(','))
                        {
                            var relateModel = new GoodsCategoryRelation()
                            {
                                GoodsId = goods.Id,
                                CategoryId = cId.ToGuid()
                            };
                            _goodsCategoryService.InsetCategoryRelation(relateModel);
                        }

                    //佣金
                    _currencyService.DeleteByConditon<GoodsCommission>(c => c.GoodsId.Equals(goods.Id));
                    for (int i = 0; i < postGoods.Commission.Length; i++)
                    {
                        _currencyService.Create(new GoodsCommission
                        {
                            Id = KeyGenerator.GetGuidKey(),
                            GoodsId = goods.Id,
                            Level = i + 1,
                            Value = postGoods.Commission[i]
                        });
                    }
                }

                //处理主图图片关联
                _storageFileService.ReplaceFile(goods.Id, MallModule.Key, MallModule.DisplayName, postGoods.MainImage, MainImage);

            }

            return Json(result);
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageGoodsKey })]
        public ActionResult Delete(Guid goodsId)
        {
            var result = new DataJsonResult();
            if (!_goodsService.RecoveryGoods(goodsId))
            {
                result.ErrorMessage = "未知异常，删除失败";
            }

            return Json(result);
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageGoodsKey })]
        public ActionResult BatchDelete(List<Guid> goodsIds)
        {
            var result = new DataJsonResult();
            if (!_goodsService.BatchRecoveryGoods(goodsIds))
            {
                result.ErrorMessage = "未知异常，删除失败";
            }

            return Json(result);
        }


        public ActionResult GetGoodsAttribute(Guid goodsTypeId)
        {
            var result = new DataJsonResult();

            result.Data = _currencyService.GetList<GoodsAttribute>(a => a.GoodsTypeId.Equals(goodsTypeId));

            return Json(result);
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageGoodsKey })]
        public ActionResult InSale(Guid id)
        {
            var result = new DataJsonResult();
            if (_goodsService.SetGoodsStatus(id, GoodsStatus.InSale))
                result.Success = true;
            else
            {
                result.Success = false;
                result.ErrorMessage = "上架失败";
            }
            return Json(result);
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageGoodsKey })]
        public ActionResult NotInSale(Guid id)
        {
            var result = new DataJsonResult();
            if (_goodsService.SetGoodsStatus(id, GoodsStatus.NotInSale))
                result.Success = true;
            else
            {
                result.Success = false;
                result.ErrorMessage = "下架失败";
            }
            return Json(result);
        }

        #region 团购相关处理
        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageGoodsKey })]
        public ActionResult GrouponValidate(Guid goodsId)
        {
            var result = new DataJsonResult();

            var goods = _currencyService.GetSingleById<Goods>(goodsId);
            if (goods == null || goods.Status == GoodsStatus.NotInSale)
                result.ErrorMessage = "该商品不存在";
            else if (goods.IsGroupon && goods.GrouponEndTime > DateTime.Now)
                result.ErrorMessage = "该商品已在团购设置中";
            else if (goods.Stock == 0)
                result.ErrorMessage = "该商品的库存为0，不能参加团购.请先补货并更新库存信息";
            else if (goods.GrouponPrice == 0)
                result.ErrorMessage = "该商品的团购价尚未设置";

            return Json(result);
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageGoodsKey })]
        public ActionResult GrouponJoin(Guid goodsId)
        {
            var result = new DataJsonResult();

            var goods = _currencyService.GetSingleById<Goods>(goodsId);
            if (goods == null || goods.Status == GoodsStatus.NotInSale)
                result.ErrorMessage = "该商品不存在";
            else if (goods.IsGroupon && goods.GrouponEndTime > DateTime.Now)
                result.ErrorMessage = "该商品已在团购设置中";
            else
            {
                string startTime = Request.Get("startTime");
                string endTime = Request.Get("endTime");
                if (string.IsNullOrWhiteSpace(startTime) || string.IsNullOrWhiteSpace(endTime))
                    result.ErrorMessage = "请设置团购开始时间和结束时间";
                else
                {
                    goods.IsGroupon = true;
                    goods.GrouponStartTime = Convert.ToDateTime(startTime);
                    goods.GrouponEndTime = Convert.ToDateTime(endTime);
                    _currencyService.Update(goods);
                    result.Data = new
                    {
                        ToGrouponSeconds =
                            goods.GrouponStartTime > DateTime.Now
                                ? (goods.GrouponStartTime - DateTime.Now).TotalSeconds
                                : 0,
                        GrouponingSeconds = (goods.GrouponEndTime - DateTime.Now).TotalSeconds,
                    };
                }
            }
            return Json(result);
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageGoodsKey })]
        public ActionResult GrouponExit(Guid goodsId)
        {
            var result = new DataJsonResult();

            var goods = _currencyService.GetSingleById<Goods>(goodsId);
            if (goods == null || goods.Status == GoodsStatus.NotInSale)
                result.ErrorMessage = "该商品不存在";
            else if (!goods.IsGroupon)
                result.ErrorMessage = "该商品不在团购活动中";
            else
            {
                goods.IsGroupon = false;
                if (!_currencyService.Update(goods))
                    result.ErrorMessage = "设置失败";
            }

            return Json(result);
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageGoodsKey })]
        public ActionResult GroupList()
        {
            return View();
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageGoodsKey })]
        public ActionResult GroupListOnPage()
        {
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

            var goodsNo = Request.Get("extra_search[GoodsNo]");
            var checkGoodsNo = string.IsNullOrWhiteSpace(goodsNo);

            var status = Request.Get("extra_search[Status]");
            var checkStatus = string.IsNullOrWhiteSpace(status);

            Expression<Func<Models.Goods, bool>> expression =
                l => (l.IsGroupon) && (checkName || l.Name.ToString().Contains(name)) &&
                     (checkGoodsNo || l.GoodsNo.Contains(goodsNo)) &&
                     (checkStatus || ((int)l.Status).ToString().Equals(status)) && l.Status != GoodsStatus.Delete;

            //分页查询
            var list = _currencyService.GetListPaged<Goods>(pageIndex, pageSize, expression, out totalCount, new OrderModelField { PropertyName = sortColumn, IsDesc = isDesc }).Select(x => new ListShortGoodsModel(x));

            result.data = list;
            result.recordsTotal = totalCount;
            result.recordsFiltered = totalCount;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 导出团购商品
        /// </summary>
        /// <returns></returns>
        [AdminAuthorize(PermissionsArray = new[] { Permissions.GoodsToExeclKey })]
        public FileResult GroupGoodsToExecl()
        {
            //取查询条件
            var name = Request.Get("Name");
            var checkName = string.IsNullOrWhiteSpace(name);

            var goodsNo = Request.Get("GoodsNo");
            var checkGoodsNo = string.IsNullOrWhiteSpace(goodsNo);

            var status = Request.Get("Status");
            var checkStatus = string.IsNullOrWhiteSpace(status);

            Expression<Func<Models.Goods, bool>> expression =
                l => (checkName || l.Name.ToString().Contains(name)) &&
                     (checkGoodsNo || l.GoodsNo.Contains(goodsNo)) &&
                     (checkStatus || ((int)l.Status).ToString().Equals(status)) && l.Status != GoodsStatus.Delete && l.IsGroupon;

            var goods = _goodsService.GetList(expression).Select(x => new ListShortGoodsModel(x));

            //创建Excel文件的对象  
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();

            //添加一个sheet  
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");
            if (goods.Any())
            {
                //给sheet1添加第一行的头部标题  
                NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
                row1.CreateCell(0).SetCellValue("商品名称");
                row1.CreateCell(1).SetCellValue("货号");
                row1.CreateCell(2).SetCellValue("价格");
                row1.CreateCell(3).SetCellValue("原价");
                row1.CreateCell(4).SetCellValue("状态");
                row1.CreateCell(5).SetCellValue("库存");
                row1.CreateCell(6).SetCellValue("销量");
                row1.CreateCell(7).SetCellValue("新品");
                row1.CreateCell(8).SetCellValue("热卖");
                row1.CreateCell(9).SetCellValue("精品");
                row1.CreateCell(10).SetCellValue("团购状态");
                var i = 0;
                foreach (var good in goods)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(good.Name);
                    rowtemp.CreateCell(1).SetCellValue(good.GoodsNo);
                    rowtemp.CreateCell(2).SetCellValue(good.ShopPrice.ToString(CultureInfo.InvariantCulture));
                    rowtemp.CreateCell(3).SetCellValue(good.OriginalPrice.ToString(CultureInfo.InvariantCulture));
                    rowtemp.CreateCell(4).SetCellValue(good.Status.Description());
                    rowtemp.CreateCell(5).SetCellValue(good.Stock);
                    rowtemp.CreateCell(6).SetCellValue(good.SalesVolume);
                    rowtemp.CreateCell(7)
                        .SetCellValue(good.IsNew ? "是" : "否");
                    rowtemp.CreateCell(8).SetCellValue(good.IsHot ? "是" : "否");
                    rowtemp.CreateCell(9).SetCellValue(good.IsBest ? "是" : "否");
                    var groupStatus = "";
                    var star = good.GrouponStartTime;
                    var end = good.GrouponEndTime;
                    var dateNow = DateTime.Now;
                    if (star > dateNow)
                    {
                        groupStatus = "未开始";
                    }
                    else if (end < dateNow)
                    {
                        groupStatus = "已结束";
                    }
                    else
                    {
                        groupStatus = "进行中";
                    }
                    rowtemp.CreateCell(10).SetCellValue(groupStatus);

                    i++;
                }
            }
            // 写入到客户端   
            var ms = new MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            var dt = DateTime.Now;
            var dateTime = dt.ToString("yyMMddHHmmssfff");
            var fileName = "团购商品列表" + dateTime + ".xls";
            return File(ms, "application/vnd.ms-excel", fileName);
        }
        #endregion

        #region 评论相关
        [AdminAuthorize]
        public ActionResult EvaluateList(Guid goodsId)
        {
            ViewBag.SourceId = goodsId;
            return View();
        }


        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageGoodsKey })]
        public ActionResult EvaluateListOnPage(Guid goodsId)
        {
            var result = new DataJsonResult();
            var pageIndex = Request.Get("pageIndex").To(1);
            var pageSize = Request.Get("pageSize").To(10);

            int totalCount = 0;
            var evaluates = _goodsService.GetGoodsEvaluatesListByPage(goodsId, pageIndex, pageSize, out totalCount);

            result.Data = new { TotalCount = totalCount, Evaluates = evaluates };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 商品导出
        [AdminAuthorize(PermissionsArray = new[] { Permissions.GoodsToExeclKey })]
        public FileResult GoodsToExecl()
        {
            //取查询条件
            var name = Request.Get("Name");
            var checkName = string.IsNullOrWhiteSpace(name);

            var goodsNo = Request.Get("GoodsNo");
            var checkGoodsNo = string.IsNullOrWhiteSpace(goodsNo);

            var status = Request.Get("Status");
            var checkStatus = string.IsNullOrWhiteSpace(status);

            var categoryId = Request.Get("CategoryId");
            var checkCategoryId = string.IsNullOrWhiteSpace(categoryId);

            Expression<Func<Models.GoodsCategoryNameView, bool>> expression =
                l => (checkName || l.Name.ToString().Contains(name)) &&
                     (checkGoodsNo || l.GoodsNo.Contains(goodsNo)) &&
                     (checkStatus || ((int)l.Status).ToString().Equals(status)) &&
                     (checkCategoryId || l.MergerId.Contains(categoryId)) && l.Status != GoodsStatus.Delete;

            //分页查询
            var goods = _currencyService.GetList<GoodsCategoryNameView>(expression, new OrderModelField { PropertyName = "CreateTime", IsDesc = true });


            //创建Excel文件的对象  
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();

            //添加一个sheet  
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");
            if (goods.Any())
            {
                //给sheet1添加第一行的头部标题  
                NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
                row1.CreateCell(0).SetCellValue("商品名称");
                row1.CreateCell(1).SetCellValue("货号");
                row1.CreateCell(2).SetCellValue("价格");
                row1.CreateCell(3).SetCellValue("原价");
                row1.CreateCell(4).SetCellValue("状态");
                row1.CreateCell(5).SetCellValue("库存");
                row1.CreateCell(6).SetCellValue("销量");
                row1.CreateCell(7).SetCellValue("新品");
                row1.CreateCell(8).SetCellValue("热卖");
                row1.CreateCell(9).SetCellValue("精品");
                var i = 0;
                foreach (var good in goods)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(good.Name);
                    rowtemp.CreateCell(1).SetCellValue(good.GoodsNo);
                    rowtemp.CreateCell(2).SetCellValue(good.ShopPrice.ToString(CultureInfo.InvariantCulture));
                    rowtemp.CreateCell(3).SetCellValue(good.OriginalPrice.ToString(CultureInfo.InvariantCulture));
                    rowtemp.CreateCell(4).SetCellValue(good.Status.Description());
                    rowtemp.CreateCell(5).SetCellValue(good.Stock);
                    rowtemp.CreateCell(6).SetCellValue(good.SalesVolume);
                    rowtemp.CreateCell(7)
                        .SetCellValue(good.IsNew ? "是" : "否");
                    rowtemp.CreateCell(8).SetCellValue(good.IsHot ? "是" : "否");
                    rowtemp.CreateCell(9).SetCellValue(good.IsBest ? "是" : "否");
                    i++;
                }
            }
            // 写入到客户端   
            var ms = new MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            var dt = DateTime.Now;
            var dateTime = dt.ToString("yyMMddHHmmssfff");
            var fileName = "商品列表" + dateTime + ".xls";
            return File(ms, "application/vnd.ms-excel", fileName);
        }

        #endregion


    }
}