/* 
    ======================================================================== 
        File name：        CarouselDbContext
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/6/22 17:36:33
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using BntWeb.Carousel.Models;
using BntWeb.Data;

namespace BntWeb.Carousel
{
    public class CarouselDbContext : BaseDbContext
    {
        public DbSet<CarouselItem> CarouselItems { get; set; }

        public DbSet<CarouselGroup> CarouselGroups { get; set; }
    }
}