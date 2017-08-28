/* 
    ======================================================================== 
        File name：		ICartService
        Module:			
        Author：		Daniel.Wu（wujb）
        Create Time：		2016/7/6 11:05:43
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BntWeb.Mall.Models;

namespace BntWeb.Mall.Services
{
    public interface ICartService :IDependency
    {
        Cart Create(Cart model);

        List<Cart> GetList(string memberId);

        void SetGoodsInvalidByGoodsId(Guid goodsId);
        void SetGoodsInvalidBySingleGoodsId(Guid singleGoodsId);
        /// <summary>
        /// 清除用户购物车无效商品
        /// </summary>
        /// <param name="memberId"></param>
        void DeleteMemberCartInvalidGoods(string memberId);
    }
}