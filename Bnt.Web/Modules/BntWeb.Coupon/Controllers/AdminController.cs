using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using BntWeb.Coupon.Models;
using BntWeb.Coupon.Services;
using BntWeb.Coupon.ViewModels;
using BntWeb.Data;
using BntWeb.Data.Services;

using BntWeb.FileSystems.Media;
using BntWeb.MemberBase.Models;
using BntWeb.Mvc;
using BntWeb.Security;
using BntWeb.SystemMessage;
using BntWeb.SystemMessage.Models;
using BntWeb.SystemMessage.Services;
using BntWeb.Utility.Extensions;
using BntWeb.Validation;
using BntWeb.Web.Extensions;

namespace BntWeb.Coupon.Controllers
{
    public class AdminController : Controller
    {
        private readonly ICurrencyService _currencyService;
        private readonly ISystemMessageService _systemMessageService;
        private readonly ICouponService _couponService;
        /// <summary>
        /// 构造
        /// </summary>
        public AdminController(ICurrencyService currencyService, ISystemMessageService systemMessageService, ICouponService couponService)
        {
            _couponService = couponService;
            _systemMessageService = systemMessageService;
            _currencyService = currencyService;
        }

        [AdminAuthorize]
        public ActionResult List()
        {
            return View();
        }

        [AdminAuthorize]
        public ActionResult ListOnPage()
        {
            var result = new DataTableJsonResult();

            //取参数值
            int draw, pageIndex, pageSize, totalCount;
            string sortColumn;
            bool isDesc;
            Request.GetDatatableParameters(out draw, out pageIndex, out pageSize, out sortColumn, out isDesc);
            result.draw = draw;


            //分页查询
            var list = _currencyService.GetListPaged<Models.Coupon>(pageIndex, pageSize, out totalCount, new OrderModelField { PropertyName = sortColumn, IsDesc = isDesc });

            result.data = list;
            result.recordsTotal = totalCount;
            result.recordsFiltered = totalCount;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AdminAuthorize]
        public ActionResult Create()
        {
            Models.Coupon coupon = new Models.Coupon();
            coupon.Id = Guid.Empty;
            coupon.ExpiryType = ExpiryType.ExpiryByDay;
            coupon.CouponType = CouponType.FullCut;
            return View("Edit", coupon);
        }

        [AdminAuthorize]
        public ActionResult Edit(Guid id)
        {
            Models.Coupon coupon = null;
            if (id == Guid.Empty)
            {
                coupon = new Models.Coupon { Id = Guid.Empty };
            }
            else
            {
                coupon = _currencyService.GetSingleById<Models.Coupon>(id);
                Argument.ThrowIfNull(coupon, "优惠券信息不存在");
            }
            return View(coupon);
        }


        [AdminAuthorize]
        public ActionResult EditOnPost(ViewCoupon model)
        {
            var result = new DataJsonResult();
            try
            {
                Models.Coupon coupon = null;
                if (model.Id != Guid.Empty)
                    coupon = _currencyService.GetSingleById<Models.Coupon>(model.Id);
                bool isNew = coupon == null;
                if (isNew)
                {
                    coupon = new Models.Coupon
                    {
                        Id = KeyGenerator.GetGuidKey(),
                        CreateTime = DateTime.Now,
                        Status = Models.CouponStatus.Enable
                    };
                }
                coupon.CouponType = model.CouponType;
                if (model.CouponType == CouponType.FullCut)
                {
                    if(model.Money <=0)
                        throw new Exception("优惠金额不能小于0");
                    coupon.Minimum = model.Minimum;
                    coupon.Money = model.Money;
                }
                else
                {
                    if (model.Discount < 0.1M || model.Discount>10)
                        throw new Exception("优惠折扣必需在0.1-10之间");
                    coupon.Discount = model.Discount;
                }
                coupon.Title = model.Title;
                coupon.Quantity = model.Quantity;
                coupon.ExpiryType = model.ExpiryType;
                

                if (model.ExpiryType == ExpiryType.ExpiryByDay)
                {
                    coupon.ExpiryDay = model.ExpiryDay;
                    coupon.StartTime = null;
                    coupon.EndTime = null;
                }
                else
                {
                    coupon.ExpiryDay = 0;
                    coupon.StartTime = Convert.ToDateTime(model.StartTime).DayZero();
                    coupon.EndTime = Convert.ToDateTime(model.EndTime).DayEnd();
                    if (coupon.StartTime > coupon.EndTime)
                    {
                        throw new Exception("开始时间不能大于结束时间");
                    }
                }
                coupon.Describe = model.Describe;

                if (isNew)
                {
                    if (!_currencyService.Create(coupon))
                    {
                        throw new Exception("保存出错");
                    }
                }
                else
                {
                    if (!_currencyService.Update(coupon))
                    {
                        throw new Exception("保存出错");
                    }
                }
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.Message;
            }

            return Json(result);
        }

        [AdminAuthorize]
        public ActionResult Delete(Guid id)
        {
            var result = new DataJsonResult();
            _currencyService.DeleteByConditon<Models.Coupon>(c => c.Id == id);
            return Json(result);
        }
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [AdminAuthorize]
        public ActionResult GetUserByUserName(string userName)
        {
            var result = new DataJsonResult();

            var member = _currencyService.GetSingleByConditon<Member>(me => me.UserName == userName);
            if (member == null)
                result.ErrorMessage = "用户不存在";
            else
                result.Data = member.Id;
            return Json(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AdminAuthorize]
        public ActionResult SendCoupon(Guid id)
        {
            var model = _currencyService.GetSingleById<Models.Coupon>(id);

          //  var memberTypes = _currencyService.GetList<MemberType>(me => me.Status == 1);
           // ViewBag.MemberTypes = memberTypes;

            return View(model);
        }

        /// <summary>
        /// 发放优惠券
        /// </summary>
        /// <returns></returns>
        [AdminAuthorize]
        public ActionResult SendCouponOnPost(SendCoupon model)
        {
            var result = new DataJsonResult();
            try
            {
                //发放或赠送的不计算数量
                var coupon = _currencyService.GetSingleById<Models.Coupon>(model.CouponId);
                if (coupon == null)
                    throw new Exception("优惠券Id不合法");

                //消息推送内容
                var pushTargetType = (int)PushTargetType.Coupon;
                Dictionary<string, string> pushDic = new Dictionary<string, string>();
                pushDic.Add("PushTargetType", pushTargetType.ToString());
                var title = "系统赠送优惠券";
                var content = "";
                if (coupon.CouponType == CouponType.Discount)
                    content = $"恭喜您，获得系统赠送的[{coupon.Discount}折]的折扣券，请尽快使用！";
                else
                    content = $"恭喜您，获得系统赠送的[满{coupon.Minimum}减{coupon.Money}]的满减券，请尽快使用！";

                using (TransactionScope scope = new TransactionScope())
                {
                    if (model.SentType == 1)
                    {
                        //分类别发放
                        List<Member> members=null;
                        if (model.MemberType ==0)
                            members = _currencyService.GetList<Member>(me => me.UserType == Security.Identity.UserType.Member);
                        //else
                        //    members = _currencyService.GetList<Member>(me => me.MemberType == model.MemberType && me.UserType == Security.Identity.UserType.Member);

                        if (members == null || members.Count == 0)
                            throw new Exception("该分类没有会员");
                        foreach (var item in members)
                        {

                            if (!(SendCouponToMember(item, coupon)))
                                throw new Exception("发放失败");
                        }

                        //消息通知发送
                        if (!_systemMessageService.CreatePushSystemMessage(title, content, content,
                                    null, null, pushDic, "BackSend", SystemMessageModule.Key, MessageCategory.System))
                        {
                            throw new Exception("发送群消息失败");
                        }
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(model.UserName))
                            throw new Exception("请输入用户名");

                        var member = _currencyService.GetSingleByConditon<Member>(me => me.UserName == model.UserName);
                        if (member == null)
                            throw new Exception("会员不存在");

                        if (!(SendCouponToMember(member, coupon)))
                            throw new Exception("发放失败");

                        //消息通知发送
                        if (!_systemMessageService.CreatePushSystemMessage(title, content, content,
                                    member.Id, null, pushDic, "BackSend", SystemMessageModule.Key, MessageCategory.System))
                        {
                            throw new Exception("发送群消息失败");
                        }
                    }

                    scope.Complete();
                }
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.Message;
            }

            //发放
            return Json(result);
        }

        /// <summary>
        /// 发放优惠券到指定会员
        /// </summary>
        /// <param name="member"></param>
        /// <param name="coupon"></param>
        /// <returns></returns>
        private bool SendCouponToMember(Member member, Models.Coupon coupon)
        {
            CouponRelation couponRel = new CouponRelation
            {
                Id = KeyGenerator.GetGuidKey(),
                CouponId = coupon.Id,
                CouponCode = _couponService.CreateCouponCode(),
                MemberId = member.Id,
                CreateTime = DateTime.Now,
                IsUsed = false,
                FromType = CouponRelation.EnumFromType.Send
            };
            //有效期
            if (coupon.ExpiryType == ExpiryType.ExpiryByDay)
            {
                couponRel.StartTime = DateTime.Now;
                if (coupon.ExpiryDay != 0)
                    couponRel.EndTime = DateTime.Now.AddDays(Convert.ToInt32(coupon.ExpiryDay));
                else
                    couponRel.EndTime = DateTime.MaxValue;
            }
            else
            {
                couponRel.StartTime = coupon.StartTime;
                couponRel.EndTime = coupon.EndTime;
            }
            return _currencyService.Create(couponRel);
        }
    }
}