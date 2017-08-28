using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace BntWeb.Coupon.Services
{
    public interface ICouponService : IDependency
    {
        /// <summary>
        /// 优惠券
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="expression"></param>
        /// <param name="orderByExpression"></param>
        /// <param name="isDesc"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        List<Models.Coupon> GetListPaged<TKey>(int pageIndex, int pageSize,
            Expression<Func<Models.Coupon, bool>> expression, Expression<Func<Models.Coupon, TKey>> orderByExpression,
            bool isDesc, out int totalCount);

        /// <summary>
        /// 我的优惠券
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="expression"></param>
        /// <param name="orderByExpression"></param>
        /// <param name="isDesc"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        List<Models.CouponRelation> MyCouponList(int pageIndex, int pageSize, string memberId, out int totalCount);

        /// <summary>
        /// 生成优惠券码
        /// </summary>
        /// <param name="codeLen"></param>
        /// <returns></returns>
        string CreateCouponCode(int codeLen = 10);

        /// <summary>
        /// 根据优惠券码回去优惠券
        /// </summary>
        /// <param name="couponCode"></param>
        /// <returns></returns>
        Models.Coupon GetCouponByCode(string couponCode);

        List<Models.CouponRelation> MyCouponListInfo(decimal fullPrice, int type, string memberId, out int totalCount,
            int pageIndex = 1,
            int pageSize = 10);
    }
}