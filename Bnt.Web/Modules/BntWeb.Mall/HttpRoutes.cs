using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using BntWeb.Mvc.Routes;
using BntWeb.WebApi.Routes;

namespace BntWeb.Mall
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
                                                        RouteTemplate = "Api/v1/Mall/GoodsEvaluates",
                                                        Defaults = new
                                                        {
                                                            area = MallModule.Area,
                                                            controller = "Evaluate",
                                                            action = "GetEvaluatesList"
                                                        }
                                                    },
                            new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Mall/GoodsEvaluates/Top",
                                                        Defaults = new
                                                        {
                                                            area = MallModule.Area,
                                                            controller = "Evaluate",
                                                            action = "GetNew9EvaluatesMember"
                                                        }
                                                    },
                             new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Mall/Home",
                                                        Defaults = new
                                                        {
                                                            area = MallModule.Area,
                                                            controller = "Goods",
                                                            action = "GetMallHomeCategroyGoods"
                                                        }
                                                    },
                             new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Mall/Goods/Recommend",
                                                        Defaults = new
                                                        {
                                                            area = MallModule.Area,
                                                            controller = "Goods",
                                                            action = "GetRecommendGoods"
                                                        }
                                                    },
                             new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Mall/Goods",
                                                        Defaults = new
                                                        {
                                                            area = MallModule.Area,
                                                            controller = "Goods",
                                                            action = "GetGoods"
                                                        }
                                                    },
                             new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Mall/GroupGoods",
                                                        Defaults = new
                                                        {
                                                            area = MallModule.Area,
                                                            controller = "Goods",
                                                            action = "GetGroupGoods"
                                                        }
                                                    },
                             new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Mall/PresellGoods",
                                                        Defaults = new
                                                        {
                                                            area = MallModule.Area,
                                                            controller = "Goods",
                                                            action = "GetPresellGoods"
                                                        }
                                                    },
                             new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Mall/OrderCalculation",
                                                        Defaults = new
                                                        {
                                                            area = MallModule.Area,
                                                            controller = "OrderCalculation"
                                                        }
                                                    },
                             new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Mall/Order/{id}",
                                                        Defaults = new
                                                        {
                                                            area = MallModule.Area,
                                                            controller = "Order"
                                                        }
                                                    },
                             new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Mall/Order",
                                                        Defaults = new
                                                        {
                                                            area = MallModule.Area,
                                                            controller = "Order"
                                                        }
                                                    },
                             new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Mall/Goods/Attribute",
                                                        Defaults = new
                                                        {
                                                            area = MallModule.Area,
                                                            controller = "Goods",
                                                            action = "GetGoodsAttrubiteAndSingleGoods"
                                                        }
                                                    },
                             new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Mall/Goods/FilterCriterias",
                                                        Defaults = new
                                                        {
                                                            area = MallModule.Area,
                                                            controller = "Goods",
                                                            action = "GetFilterCriterias"
                                                        }
                                                    },
                             new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Mall/Cart/Clear",
                                                        Defaults = new
                                                        {
                                                            area = MallModule.Area,
                                                            controller = "Cart",
                                                            action="ClearCart"
                                                        }
                                                    },
                             new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Mall/Cart/",
                                                        Defaults = new
                                                        {
                                                            area = MallModule.Area,
                                                            controller = "Cart"
                                                        }
                                                    },
                             new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Mall/Cart/{cartId}",
                                                        Defaults = new
                                                        {
                                                            area = MallModule.Area,
                                                            controller = "Cart"
                                                        }
                                                    },
                             new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Mall/GoodsCategory",
                                                        Defaults = new
                                                        {
                                                            area = MallModule.Area,
                                                            controller = "GoodsCategory",
                                                            action = "GetGoodsCategory"
                                                        }
                                                    },
                             new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Mall/Collect",
                                                        Defaults = new
                                                        {
                                                            area = MallModule.Area,
                                                            controller = "Collect"
                                                        }
                                                    },
                              new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Mall/Collect/{goodsId}",
                                                        Defaults = new
                                                        {
                                                            area = MallModule.Area,
                                                            controller = "Collect"
                                                        }
                                                    },
                              new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Mall/Browse",
                                                        Defaults = new
                                                        {
                                                            area = MallModule.Area,
                                                            controller = "Browse"
                                                        }
                                                    },
                              new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Mall/Browse/{goodsId}",
                                                        Defaults = new
                                                        {
                                                            area = MallModule.Area,
                                                            controller = "Browse"
                                                        }
                                                    }
                         };
        }

    }
}
