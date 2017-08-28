using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BntWeb.Mvc.Routes;
using BntWeb.WebApi.Routes;

namespace BntWeb.Coupon
{
    public class HttpRoutes: IHttpRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[] {
                            
                            new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Coupons",
                                                        Defaults = new
                                                        {
                                                            area = CouponModule.Area,
                                                            controller = "Coupon",
                                                            action="GetCouponList"
                                                        }
                                                    },
                            new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Coupon/Receive/{couponId}",
                                                        Defaults = new
                                                        {
                                                            area = CouponModule.Area,
                                                            controller = "Coupon",
                                                            action="ReceiveCoupon"
                                                        }
                                                    },
                            new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Coupon/MyCoupon",
                                                        Defaults = new
                                                        {
                                                            area = CouponModule.Area,
                                                            controller = "Coupon",
                                                            action="MyCoupon"
                                                        }
                                                    },
                            new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Coupon/MyCouponList",
                                                        Defaults = new
                                                        {
                                                            area = CouponModule.Area,
                                                            controller = "Coupon",
                                                            action="MyCouponList"
                                                        }
                                                    }
                            
                         };
        }

    }
}