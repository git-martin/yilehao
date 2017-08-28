using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using BntWeb.Data.Services;
using BntWeb.FileSystems.Media;
using BntWeb.Logging;
using BntWeb.Security;

namespace BntWeb.Coupon.Services
{
    public class CouponService : ICouponService
    {
        public ILogger Logger { get; set; }
        public CouponService()
        {
        }

        public List<Models.Coupon> GetListPaged<TKey>(int pageIndex, int pageSize, Expression<Func<Models.Coupon, bool>> expression, Expression<Func<Models.Coupon, TKey>> orderByExpression, bool isDesc, out int totalCount)
        {
            using (var dbContext = new CouponDbContext())
            {
                var query = dbContext.Coupon.Where(expression);
                totalCount = query.Count();
                if (isDesc)
                    query = query.OrderByDescending(orderByExpression);
                else
                    query = query.OrderBy(orderByExpression);

                var list = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                return list;
            }
        }


        public List<Models.CouponRelation> MyCouponList(int pageIndex, int pageSize, string memberId, out int totalCount)
        {
            using (var dbContext = new CouponDbContext())
            {
                var query = dbContext.CouponRelation.Include(c => c.Coupon).Where(cr => cr.MemberId == memberId && cr.Coupon.Status == Models.CouponStatus.Enable);
                totalCount = query.Count();
                query = query.OrderByDescending(cr => cr.CreateTime);

                var list = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                return list;
            }
        }

        public List<Models.CouponRelation> MyCouponListInfo(decimal fullPrice, int type, string memberId, out int totalCount, int pageIndex = 1,
            int pageSize = 10)
        {
            using (var dbContext = new CouponDbContext())
            {
                var query = dbContext.CouponRelation.Include(c => c.Coupon).Where(cr => cr.MemberId == memberId && cr.Coupon.Status == Models.CouponStatus.Enable);
                if (fullPrice > 0)
                {
                    query = query.Where(cr => cr.Coupon.Minimum <= fullPrice);
                }
                if (type == 0)
                {
                    query = query.Where(cr => cr.IsUsed);
                }
                else if (type == 2)
                {
                    query = query.Where(cr => cr.EndTime < DateTime.Now);
                }
                else
                {
                    query = query.Where(cr => cr.StartTime <= DateTime.Now && cr.EndTime >= DateTime.Now && !cr.IsUsed);
                }

                totalCount = query.Count();
                query = query.OrderByDescending(cr => cr.CreateTime);

                var list = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                return list;
            }
        }

        /// <summary>
        /// 生成优惠券码
        /// </summary>
        /// <param name="codeLen"></param>
        /// <returns></returns>
        public string CreateCouponCode(int codeLen = 10)
        {
            string codeStr = "0,1,2,3,4,5,6,7,8,9,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";
            string[] arr = codeStr.Split(',');
            string code = "";
            Random rand = new Random(unchecked((int)DateTime.Now.Ticks));
            for (int i = 0; i < codeLen; i++)
            {
                var randValue = rand.Next(0, arr.Length - 1);

                code += arr[randValue];
            }
            //判断数据库是否存在，wujb
            var exists = false;
            using (var dbContext = new CouponDbContext())
            {
                if (dbContext.CouponRelation.FirstOrDefault(me => me.CouponCode == code) != null)
                {
                    exists = true;
                }
            }
            return !exists ? code : CreateCouponCode(codeLen);
        }

        /// <summary>
        /// 根据优惠券码回去优惠券
        /// </summary>
        /// <param name="couponCode"></param>
        /// <returns></returns>
        public Models.Coupon GetCouponByCode(string couponCode)
        {
            using (var dbContext = new CouponDbContext())
            {
                var query = from c in dbContext.Coupon
                            join cr in dbContext.CouponRelation
                                on c.Id equals cr.CouponId
                            where cr.CouponCode == couponCode
                            select c;

                return query.FirstOrDefault();
            }
        }
    }
}