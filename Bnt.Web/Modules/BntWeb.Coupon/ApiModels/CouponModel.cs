using System;
using BntWeb.Coupon.Models;

namespace BntWeb.Coupon.ApiModels
{
    public class CouponModel
    {
        public Guid Id { set; get; }
        public string Title { get; set; }

        public CouponType CouponType { get; set; }

        public decimal Money { get; set; }


        public string Describe { get; set; }

        /// <summary>
        /// 有效期类型，1指定天，2指定日期
        /// </summary>
        public ExpiryType ExpiryType { set; get; }

        public decimal Minimum { get; set; }
        public decimal Discount { get; set; }

        public int? ExpiryDay { set; get; }

        public string StartTime { get; set; }


        public string EndTime { get; set; }

        /// <summary>
        /// 是否已经领用
        /// </summary>
        public bool IsOwn { set; get; }


        public CouponModel()
        {
            
        }

        public CouponModel(Models.Coupon model)
        {
            Id = model.Id;
            Title = model.Title;
            Describe = model.Describe;
            ExpiryType = model.ExpiryType;
            ExpiryDay = model.ExpiryDay;
            CouponType = model.CouponType;
            if (model.CouponType == CouponType.FullCut)
            {
                Money = model.Money;
                Minimum = model.Minimum;
            }
            else
            {
                Discount = model.Discount;
            }

            if (ExpiryType == ExpiryType.ExpiryByDate)
            {
                StartTime = Convert.ToDateTime(model.StartTime).ToString("yyyy-MM-dd");
                EndTime = Convert.ToDateTime(model.EndTime).ToString("yyyy-MM-dd");
            }
        }
        
    }


    public class CouponRelationModel
    {
        public string CouponCode { set; get; }
        public CouponType CouponType { get; set; }
        public Guid Id { set; get; }
        public string Title { get; set; }


        public decimal Money { get; set; }


        public string Describe { get; set; }


        public decimal Minimum { get; set; }
        public decimal Discount { get; set; }


        public bool IsUsed { get; set; }

        public string StartTime { get; set; }


        public string EndTime { get; set; }

        public int Status { set; get; } = 0;


        public CouponRelationModel(CouponRelation model)
        {
            CouponCode = model.CouponCode;
            CouponType = model.Coupon.CouponType;
            Id = model.CouponId;
            Title = model.Coupon.Title;
            Describe = model.Coupon.Describe;
            IsUsed = model.IsUsed;
            StartTime = Convert.ToDateTime(model.StartTime).ToString("yyyy-MM-dd");
            EndTime = Convert.ToDateTime(model.EndTime).ToString("yyyy-MM-dd");
            if (model.Coupon.CouponType == CouponType.FullCut)
            {
                Money = model.Coupon.Money;
                Minimum = model.Coupon.Minimum;
            }
            else
            {
                Discount = model.Coupon.Discount;
            }

            if (model.IsUsed)
                Status = 1; //已使用
            else
            {
                if (Convert.ToDateTime(model.EndTime) < DateTime.Now)
                    Status = 2; //已失效
                else
                    Status = 0;//未使用
            }
        }
    }
}