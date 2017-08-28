using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Web.Http;
using BntWeb.Caching;
using BntWeb.Coupon.ApiModels;
using BntWeb.Coupon.Models;
using BntWeb.Coupon.Services;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.FileSystems.Media;

using BntWeb.Security.Identity;
using BntWeb.Services;

using BntWeb.WebApi.Filters;
using BntWeb.WebApi.Models;


namespace BntWeb.Coupon.ApiControllers
{

    public class CouponController : BaseApiController
    {

        private readonly ICurrencyService _currencyService;
        private readonly ICouponService _couponService;


        public CouponController(ICurrencyService currencyService, ICouponService couponService)
        {
            _currencyService = currencyService;
            _couponService = couponService;
        }


        /// <summary>
        /// 优惠券列表
        /// </summary>
        /// <param name="pageNo"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet]
        [BasicAuthentication(Forcible = false)]
        public ApiResult GetCouponList(int pageNo = 1, int limit = 10)
        {
            var result = new ApiResult();
            int totalCount;
            var coupon = _couponService.GetListPaged(pageNo, limit, c => c.Status == CouponStatus.Enable,
                c => c.CreateTime, true, out totalCount);

            var resCoupons = new List<CouponModel>();
            foreach (var item in coupon)
            {
                var resCoupon = new CouponModel(item);
                if (AuthorizedUser == null)
                    resCoupon.IsOwn = false;
                else
                {
                    //判断是否已经领取
                    var couponRelation = _currencyService.GetSingleByConditon<CouponRelation>(c => c.CouponId == item.Id && c.MemberId == AuthorizedUser.Id);
                    resCoupon.IsOwn = couponRelation != null;
                }

                resCoupons.Add(resCoupon);
            }

            var data = new
            {
                TotalCount = totalCount,
                Coupons = resCoupons
            };
            result.SetData(data);
            return result;
        }

        /// <summary>
        /// 优惠券领取
        /// </summary>
        /// <param name="couponId"></param>
        /// <returns></returns>
        [HttpPost]
        [BasicAuthentication]
        public ApiResult ReceiveCoupon(Guid couponId)
        {
            var coupon = _currencyService.GetSingleById<Models.Coupon>(couponId);
            var couponRelation = _currencyService.GetSingleByConditon<Models.CouponRelation>(c => c.CouponId == couponId && c.MemberId == AuthorizedUser.Id);
            if (coupon == null)
                throw new WebApiInnerException("0001", "优惠券Id不合法");
            if (coupon.Quantity == 0)
                throw new WebApiInnerException("0002", "优惠券已领完");
            if (couponRelation != null)
                throw new WebApiInnerException("0003", "已领取过优惠券");
            Models.CouponRelation model = new Models.CouponRelation
            {
                Id = KeyGenerator.GetGuidKey(),
                CouponId = coupon.Id,
                CouponCode = _couponService.CreateCouponCode(),
                MemberId = AuthorizedUser.Id,
                CreateTime = DateTime.Now,
                IsUsed = false,
                FromType = CouponRelation.EnumFromType.Get
            };
            //有效期
            if (coupon.ExpiryType == ExpiryType.ExpiryByDay)
            {
                model.StartTime = DateTime.Now;
                if (coupon.ExpiryDay != 0)
                    model.EndTime = DateTime.Now.AddDays(Convert.ToInt32(coupon.ExpiryDay));
                else
                    model.EndTime = DateTime.MaxValue;
            }
            else
            {
                model.StartTime = coupon.StartTime;
                model.EndTime = coupon.EndTime;
            }

            coupon.Quantity = coupon.Quantity - 1;
            _currencyService.Update(coupon);
            _currencyService.Create(model);

            return new ApiResult();
        }


        /// <summary>
        /// 我的优惠券
        /// </summary>
        /// <param name="pageNo"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet]
        [BasicAuthentication]
        public ApiResult MyCoupon(int pageNo = 1, int limit = 10)
        {
            int totalCount;

            var list = _couponService.MyCouponList(pageNo, limit, AuthorizedUser.Id, out totalCount);
            var result = new ApiResult();
            var data = new
            {
                TotalCount = totalCount,
                Coupons = list.Select(item => new CouponRelationModel(item)).ToList()
            };
            result.SetData(data);
            return result;
        }

        [HttpGet]
        [BasicAuthentication]
        public ApiResult MyCouponList(decimal fullPrice = 0, int pageNo = 1, int limit = 10, int type = 1)
        {
            int totalCount;
            var list = _couponService.MyCouponListInfo(fullPrice, type, AuthorizedUser.Id, out totalCount, pageNo, limit);
            var data = new
            {
                TotalCount = totalCount,
                List = list.Select(item => new CouponRelationModel(item)).ToList()
            };
            var result = new ApiResult();
            result.SetData(data);
            return result;
        }
    }
}