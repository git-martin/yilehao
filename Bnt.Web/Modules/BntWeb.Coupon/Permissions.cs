
using System.Collections.Generic;
using BntWeb.Security.Permissions;

namespace BntWeb.Coupon
{
    public class Permissions : IPermissionProvider
    {
        private static readonly string CategoryKey = CouponModule.DisplayName;

        public const string CouponKey = "BntWeb-Coupon-CouponList";
        public static readonly Permission Coupon = new Permission { Description = "优惠券管理", Name = CouponKey, Category = CategoryKey };

        public int Position => CouponModule.Position;

        public string Category => CategoryKey;

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                Coupon,
            };
        }
    }
}