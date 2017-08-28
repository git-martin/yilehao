/* 
    ======================================================================== 
        File name：        Routes
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/4/29 8:47:08
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/

using System.Collections.Generic;
using System.Web.Http;
using BntWeb.Mvc.Routes;
using BntWeb.WebApi.Routes;

namespace BntWeb.MemberCenter
{
    public class HttpRoutes : IHttpRouteProvider
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
                                                        RouteTemplate = "Api/v1/Member/CenterInfo",
                                                        Defaults = new
                                                        {
                                                            area = MemberCenterModule.Area,
                                                            controller = "Member",
                                                            action = "GetCenterInfo",
                                                            id = RouteParameter.Optional
                                                        }
                                                    },
                            new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Member/Commission",
                                                        Defaults = new
                                                        {
                                                            area = MemberCenterModule.Area,
                                                            controller = "Partner",
                                                            action = "GetCommission",
                                                            id = RouteParameter.Optional
                                                        }
                                                    },
                            new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Member/Partner",
                                                        Defaults = new
                                                        {
                                                            area = MemberCenterModule.Area,
                                                            controller = "Partner",
                                                            action = "GetPartner",
                                                            id = RouteParameter.Optional
                                                        }
                                                    },
                            new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Member/{id}/Bound/PhoneNumber",
                                                        Defaults = new
                                                        {
                                                            area = MemberCenterModule.Area,
                                                            controller = "Member",
                                                            action = "BoundPhoneNumber",
                                                            id = RouteParameter.Optional
                                                        }
                                                    },
                            new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Member/{id}/PhoneNumber",
                                                        Defaults = new
                                                        {
                                                            area = MemberCenterModule.Area,
                                                            controller = "Member",
                                                            action = "ChangePhoneNumber",
                                                            id = RouteParameter.Optional
                                                        }
                                                    },
                             new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Member/Address/{addressId}/Default",
                                                        Defaults = new
                                                        {
                                                            area = MemberCenterModule.Area,
                                                            controller = "MemberAddress",
                                                            action="SetDefault"
                                                        }
                                                    },
                             new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Member/Address/{addressId}",
                                                        Defaults = new
                                                        {
                                                            area = MemberCenterModule.Area,
                                                            controller = "MemberAddress"
                                                        }
                                                    },
                             new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Member/Address/",
                                                        Defaults = new
                                                        {
                                                            area = MemberCenterModule.Area,
                                                            controller = "MemberAddress"
                                                        }
                                                    },
                             new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Member/SendCode",
                                                        Defaults = new
                                                        {
                                                            area = MemberCenterModule.Area,
                                                            controller = "Sms",
                                                            action = "SendCode",
                                                            phone = RouteParameter.Optional
                                                        }
                                                    },
                             new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Member/ResetPassword",
                                                        Defaults = new
                                                        {
                                                            area = MemberCenterModule.Area,
                                                            controller = "Member",
                                                            action = "ResetPassword",
                                                            id = RouteParameter.Optional
                                                        }
                                                    },
                             new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Member/{id}/Avatar",
                                                        Defaults = new
                                                        {
                                                            area = MemberCenterModule.Area,
                                                            controller = "Member",
                                                            action = "UpdateAvatar",
                                                            id = RouteParameter.Optional
                                                        }
                                                    },
                             new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Member/{id}/Password",
                                                        Defaults = new
                                                        {
                                                            area = MemberCenterModule.Area,
                                                            controller = "Member",
                                                            action = "ChangePassword",
                                                            id = RouteParameter.Optional
                                                        }
                                                    },
                             new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Member/{id}",
                                                        Defaults = new
                                                        {
                                                            area = MemberCenterModule.Area,
                                                            controller = "Member",
                                                            id = RouteParameter.Optional
                                                        }
                                                    },
                             new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Login",
                                                        Defaults = new
                                                        {
                                                            area = MemberCenterModule.Area,
                                                            controller = "Security",
                                                            action="Login"
                                                        }
                                                    },
                             new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/LoginWithSms",
                                                        Defaults = new
                                                        {
                                                            area = MemberCenterModule.Area,
                                                            controller = "Security",
                                                            action="LoginWithSms"
                                                        }
                                                    },
                             new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/LoginWithOAuth",
                                                        Defaults = new
                                                        {
                                                            area = MemberCenterModule.Area,
                                                            controller = "Security",
                                                            action="LoginWithOAuth"
                                                        }
                                                    },
                             new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Logout",
                                                        Defaults = new
                                                        {
                                                            area = MemberCenterModule.Area,
                                                            controller = "Security",
                                                            action="Logout"
                                                        }
                                                    },
                              new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Share",
                                                        Defaults = new
                                                        {
                                                            area = MemberCenterModule.Area,
                                                            controller = "Security",
                                                            action="GetJsApiConfig"
                                                        }
                                                    }
                         };
        }

    }
}