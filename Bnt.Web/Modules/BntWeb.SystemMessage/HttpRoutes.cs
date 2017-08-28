using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using BntWeb.Mvc.Routes;
using BntWeb.WebApi.Routes;

namespace BntWeb.SystemMessage
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
            return new[]
            {
                new HttpRouteDescriptor
                {
                    Priority = 0,
                    RouteTemplate = "Api/v1/Message/Category/{category}",
                    Defaults = new
                    {
                        area = SystemMessageModule.Area,
                        controller = "SystemMessage"
                    }
                },
                new HttpRouteDescriptor
                {
                    Priority = 0,
                    RouteTemplate = "Api/v1/Message/Category/{category}",
                    Defaults = new
                    {
                        area = SystemMessageModule.Area,
                        controller = "SystemMessage",
                        action="SetReadAll"
                    }
                },
                new HttpRouteDescriptor
                {
                    Priority = 0,
                    RouteTemplate = "Api/v1/Message/{messageId}",
                    Defaults = new
                    {
                        area = SystemMessageModule.Area,
                        controller = "SystemMessage"
                    }
                },
                new HttpRouteDescriptor
                {
                    Priority = 0,
                    RouteTemplate = "Api/v1/Message",
                    Defaults = new
                    {
                        area = SystemMessageModule.Area,
                        controller = "SystemMessage"
                    }
                }
            };
        }

    }
}
