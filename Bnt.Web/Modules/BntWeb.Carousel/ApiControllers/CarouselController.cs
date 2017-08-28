using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using BntWeb.Carousel.Services;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.FileSystems.Media;
using BntWeb.Security.Identity;
using BntWeb.Services;
using BntWeb.Utility.Extensions;
using BntWeb.Validation;
using BntWeb.WebApi.Filters;
using BntWeb.WebApi.Models;

namespace BntWeb.Carousel.ApiControllers
{
    public class CarouselController : BaseApiController
    {
        private readonly ICarouselService _carouselService;
        public CarouselController(ICarouselService carouselService)
        {
            _carouselService = carouselService;
        }

        [HttpGet]
        public ApiResult Get(string key)
        {
            Argument.ThrowIfNullOrEmpty(key, "轮播组Key值");

            var result = new ApiResult();
            var data = _carouselService.LoadItemsByGroupKey(key).Select(c => new
            {
                c.SourceTitle,
                c.Summary,
                CoverImage = c.CoverImage.Simplified(),
                c.ShotUrl
            });
            result.SetData(data);
            return result;
        }
    }
}
