/* 
    ======================================================================== 
        File name：        Permissions
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/5/9 15:34:55
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/

using System.Collections.Generic;
using BntWeb.Security.Permissions;

namespace BntWeb.Carousel
{
    public class Permissions : IPermissionProvider
    {
        private static readonly string CategoryKey = CarouselModule.DisplayName;

        public const string ViewCarouselKey = "BntWeb-Carousel-ViewMember";
        public static readonly Permission ViewCarousel = new Permission { Description = "查看轮播", Name = ViewCarouselKey, Category = CategoryKey };

        public const string EditCarouselKey = "BntWeb-Carousel-EditCarousel";
        public static readonly Permission EditCarousel = new Permission { Description = "编辑轮播", Name = EditCarouselKey, Category = CategoryKey };

        public const string DeleteCarouselKey = "BntWeb-Carousel-DeleteCarouse";
        public static readonly Permission DeleteCarousel = new Permission { Description = "删除轮播", Name = DeleteCarouselKey, Category = CategoryKey };


        public int Position => CarouselModule.Position;

        public string Category => CategoryKey;

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ViewCarousel,
                EditCarousel,
                DeleteCarousel
            };
        }
    }
}