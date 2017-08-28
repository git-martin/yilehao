using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using BntWeb.Data.Services;
using BntWeb.Mall.ApiModels;
using BntWeb.Mall.Models;
using BntWeb.Mall.Services;
using BntWeb.Utility.Extensions;
using BntWeb.Validation;
using BntWeb.WebApi.Filters;
using BntWeb.WebApi.Models;

namespace BntWeb.Mall.ApiControllers
{
    public class CartController : BaseApiController
    {
        private readonly ICartService _cartService;
        private readonly ICurrencyService _currencyService;
        private readonly IGoodsService _goodsService;

        public CartController(ICartService cartService, ICurrencyService currencyService, IGoodsService goodsService)
        {
            _cartService = cartService;
            _currencyService = currencyService;
            _goodsService = goodsService;
        }

        /// <summary>
        /// 添加购物车
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [BasicAuthentication]
        public ApiResult AddCart([FromBody]CreateCartModel editModel)
        {
            if (editModel.GoodsId == null || editModel.GoodsId == Guid.Empty)
                throw new WebApiInnerException("2001", "缺少商品Id参数");
            if (editModel.SingleGoodsId == null || editModel.SingleGoodsId == Guid.Empty)
                throw new WebApiInnerException("2002", "缺少单品Id参数");
            if (editModel.Quantity < 1)
                throw new WebApiInnerException("2003", "数量错误");

            var goods = _currencyService.GetSingleById<Goods>(editModel.GoodsId);
            if (goods == null)
                throw new WebApiInnerException("2004", "无该商品信息");

            var singleGoods = _goodsService.LoadFullSingleGoods(editModel.SingleGoodsId);
            if (singleGoods == null)
                throw new WebApiInnerException("2005", "无该单品信息");

            var model = _currencyService.GetSingleByConditon<Cart>(me=>me.MemberId == AuthorizedUser.Id && me.GoodsId == editModel.GoodsId && me.SingleGoodsId==editModel.SingleGoodsId);
            if (model == null)
            {
                model = new Cart
                {
                    GoodsId = editModel.GoodsId.ToString().ToGuid(),
                    SingleGoodsId = editModel.SingleGoodsId.ToString().ToGuid(),
                    Quantity = editModel.Quantity,
                    MemberId = AuthorizedUser.Id,
                    GoodsAttribute = string.Join(",", singleGoods.Attributes.Select(me => me.AttributeValue).ToList()),
                    SingleGoodsNo = singleGoods.SingleGoodsNo,
                    Price = singleGoods.Goods.IsGroupon ? ((singleGoods.Goods.GrouponStartTime <= DateTime.Now && singleGoods.Goods.GrouponEndTime >= DateTime.Now) ? singleGoods.GrouponPrice : singleGoods.Price) : singleGoods.Price,
                    Unit = singleGoods.Unit,
                    Status = CartStatus.Normal,
                    GoodsName = goods.Name,
                    GoodsNo = goods.GoodsNo,
                    FreeShipping = goods.FreeShipping
                };

                model = _cartService.Create(model);

                if (model.Id == Guid.Empty)
                    throw new WebApiInnerException("2006", "添加失败,内部执出错");
            }
            else
            {
                model.Price = singleGoods.Goods.IsGroupon
                    ? ((singleGoods.Goods.GrouponStartTime <= DateTime.Now &&
                        singleGoods.Goods.GrouponEndTime >= DateTime.Now)
                        ? singleGoods.GrouponPrice
                        : singleGoods.Price)
                    : singleGoods.Price;
                model.Quantity = model.Quantity + editModel.Quantity;
                _currencyService.Update(model);
            }

            var result = new ApiResult();
            var data = model;

            result.SetData(data);

            return result;
        }

        /// <summary>
        /// 删除购物车
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [BasicAuthentication]
        public ApiResult DeleteCart(Guid cartId)
        {
            if (cartId == Guid.Empty)
                throw new WebApiInnerException("2011", "无效Id");

            var cart = _currencyService.GetSingleById<Cart>(cartId);
            if (cart == null)
                throw new WebApiInnerException("2012", "购物车无该商品");

           if(_currencyService.DeleteByConditon<Cart>(me=>me.Id == cartId)<1)
                throw new WebApiInnerException("2013", "内部删除错误");

            var result = new ApiResult();

            return result;
        }

        /// <summary>
        /// 编辑单个购物车
        /// </summary>
        /// <returns></returns>
        [HttpPatch]
        [BasicAuthentication]
        public ApiResult EditCart([FromBody]EditCartModel editModel)
        {
            if (editModel.CartId == Guid.Empty)
                throw new WebApiInnerException("2021", "无效Id");
            if (editModel.Quantity<1)
                throw new WebApiInnerException("2022", "无效数量");

            var cart = _currencyService.GetSingleById<Cart>(editModel.CartId);
            if (cart == null)
                throw new WebApiInnerException("2023", "购物车无该商品");

            cart.Quantity = editModel.Quantity;

            if (!_currencyService.Update(cart))
                throw new WebApiInnerException("2024", "内部保存错误");

            var result = new ApiResult();

            return result;
        }

        /// <summary>
        /// 获取购物车列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [BasicAuthentication]
        public ApiResult GetList()
        {

            var carts = _cartService.GetList(AuthorizedUser.Id);
            var result = new ApiResult();
            var data = new
            {
                Carts = carts.Select(me=>new ListCartModel(me)).ToList()
            };

            result.SetData(data);
            return result;
        }

        /// <summary>
        /// 清除购物车无效商品
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [BasicAuthentication]
        public ApiResult ClearCart()
        {
            _cartService.DeleteMemberCartInvalidGoods(AuthorizedUser.Id);
            var result = new ApiResult();
            return result;
        }
    }
}
