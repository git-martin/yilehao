/* 
    ======================================================================== 
        File name：		CartService
        Module:			
        Author：		Daniel.Wu（wujb）
        Create Time：		2016/7/6 11:06:11
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.Logging;
using BntWeb.Mall.Models;
using BntWeb.Security;
using System.Data.Entity;

namespace BntWeb.Mall.Services
{
    public class CartService : ICartService
    {
        private readonly ICurrencyService _currencyService;

        public ILogger Logger { get; set; }
        public CartService(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        public Cart Create(Cart model)
        {
            model.Id = KeyGenerator.GetGuidKey();
            model.CreateTime = DateTime.Now;
            model.Status = CartStatus.Normal;
            var result = _currencyService.Create<Cart>(model);
            if (!result)
            {
                model.Id = Guid.Empty;
            }

            return model;
        }

        public List<Cart> GetList(string memberId)
        {
            using (var dbContext = new MallDbContext())
            {
                var query =
                    dbContext.Carts.Where(me => me.MemberId == memberId && me.Status > 0)
                        .OrderByDescending(me => me.CreateTime);
                return query.ToList();
            }
        }

        public void SetGoodsInvalidByGoodsId(Guid goodsId)
        {
            using (var dbContext = new MallDbContext())
            {
                var cartList =
                   dbContext.Carts.Where(me => me.GoodsId.Equals(goodsId) && me.Status > 0).ToList();
                foreach (Cart info in cartList)
                {
                    info.Status = CartStatus.Invalid;
                }
                dbContext.SaveChanges();
            }
        }

        public void SetGoodsInvalidBySingleGoodsId(Guid singleGoodsId)
        {
            using (var dbContext = new MallDbContext())
            {
                var cartList =
                   dbContext.Carts.Where(me => me.SingleGoodsId.Equals(singleGoodsId) && me.Status > 0).ToList();
                foreach (Cart info in cartList)
                {
                    info.Status = CartStatus.Invalid;
                }
                dbContext.SaveChanges();
            }
        }

        public void DeleteMemberCartInvalidGoods(string memberId)
        {
            using (var dbContext = new MallDbContext())
            {
                var cartList =
                    dbContext.Carts.Where(me => me.MemberId == memberId && me.Status > 0)
                        .OrderByDescending(me => me.CreateTime).ToList();

                foreach (Cart cartInfo in cartList)
                {
                    var singleGoods = dbContext.SingleGoods.Include(sg => sg.Goods).FirstOrDefault(g => g.Id.Equals(cartInfo.SingleGoodsId));
                    if (singleGoods == null)
                    {
                        _currencyService.DeleteByConditon<Models.Cart>(d => d.Id == cartInfo.Id);
                    }
                    else
                    {
                        if (singleGoods.Goods.Status != GoodsStatus.InSale || singleGoods.Stock < 1
                        )
                        {
                            _currencyService.DeleteByConditon<Models.Cart>(d => d.Id == cartInfo.Id);
                        }
                        else
                        {
                            if (singleGoods.Goods.IsGroupon && singleGoods.Goods.GrouponStartTime <= DateTime.Now &&
                                singleGoods.Goods.GrouponEndTime >= DateTime.Now && singleGoods.GrouponPrice != cartInfo.Price)
                            {
                                _currencyService.DeleteByConditon<Models.Cart>(d => d.Id == cartInfo.Id);
                            }
                            else if (singleGoods.Price != cartInfo.Price)
                            {
                                _currencyService.DeleteByConditon<Models.Cart>(d => d.Id == cartInfo.Id);
                            }
                        }
                    }
                }
            }
        }

    }
}