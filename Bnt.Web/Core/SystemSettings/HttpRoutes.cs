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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using BntWeb.Environment.Configuration;
using BntWeb.Mvc.Routes;
using BntWeb.WebApi.Routes;

namespace BntWeb.Core.SystemSettings
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
                                                        RouteTemplate = "Api/v1/Settings/District/{id}/Child",
                                                        Defaults = new
                                                        {
                                                            area = SystemSettingsModule.Area,
                                                            controller = "District",
                                                            id = RouteParameter.Optional
                                                        }
                                                    },
                             new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/File/Upload",
                                                        Defaults = new
                                                        {
                                                            area = SystemSettingsModule.Area,
                                                            controller = "File",
                                                            action="SaveUploadedFile"
                                                        }
                                                    },
                             new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/File",
                                                        Defaults = new
                                                        {
                                                            area = SystemSettingsModule.Area,
                                                            controller = "File",
                                                            action="PostFile"

                                                        }
                                                    },
                             new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/SoftRelease/{softKey}",
                                                        Defaults = new
                                                        {
                                                            area = SystemSettingsModule.Area,
                                                            controller = "Release"
                                                        }
                                                    },
                             new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Captcha",
                                                        Defaults = new
                                                        {
                                                            area = SystemSettingsModule.Area,
                                                            controller = "Captcha"
                                                        }
                                                    }
                         };
        }

    }
}