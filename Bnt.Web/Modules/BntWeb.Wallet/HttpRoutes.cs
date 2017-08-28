using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using BntWeb.Mvc.Routes;
using BntWeb.WebApi.Routes;

namespace BntWeb.Wallet
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
                    RouteTemplate = "Api/v1/Wallet/SendCode",
                    Defaults = new
                    {
                        area = WalletModule.Area,
                        controller = "Sms",
                        action = "SendCode",
                        phone = RouteParameter.Optional
                    }
                },
               new HttpRouteDescriptor
                {
                    Priority = 0,
                    RouteTemplate = "Api/v1/Wallet/Cash",
                    Defaults = new
                    {
                        area = WalletModule.Area,
                        controller = "Wallet",
                        action="GetWalletCash"
                    }
                },
                new HttpRouteDescriptor
                {
                    Priority = 0,
                    RouteTemplate = "Api/v1/Wallet/Integral",
                    Defaults = new
                    {
                        area = WalletModule.Area,
                        controller = "Wallet",
                        action="GetWalletIntegral"
                    }
                },
                new HttpRouteDescriptor
                {
                    Priority = 0,
                    RouteTemplate = "Api/v1/Wallet",
                    Defaults = new
                    {
                        area = WalletModule.Area,
                        controller = "Wallet"
                    }
                },
                new HttpRouteDescriptor
                {
                    Priority = 0,
                    RouteTemplate = "Api/v1/WalletBill",
                    Defaults = new
                    {
                        area = WalletModule.Area,
                        controller = "WalletBill"
                    }
                },
                new HttpRouteDescriptor
                {
                    Priority = 0,
                    RouteTemplate = "Api/v1/Wallet/{id}",
                    Defaults = new
                    {
                        area = WalletModule.Area,
                        controller = "Wallet"
                    }
                }
            };
        }

    }
}
