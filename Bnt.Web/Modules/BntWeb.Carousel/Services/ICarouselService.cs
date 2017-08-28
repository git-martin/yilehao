/* 
    ======================================================================== 
        File name：        ICarouselService
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/6/23 9:23:52
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BntWeb.Carousel.Models;

namespace BntWeb.Carousel.Services
{
    public interface ICarouselService : IDependency
    {
        /// <summary>
        /// 获取所有轮播分组
        /// </summary>
        /// <returns></returns>
        List<CarouselGroup> LoadAllGroups();

        /// <summary>
        /// 加载所有轮播项
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<CarouselItem> LoadItemsByGroupId(Guid id);

        /// <summary>
        /// 加载所有轮播项
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        List<CarouselItem> LoadItemsByGroupKey(string key);

        /// <summary>
        /// 保存轮播项
        /// </summary>
        /// <param name="carouselItem"></param>
        /// <returns></returns>
        bool SaveCarouselItem(CarouselItem carouselItem);

        /// <summary>
        /// 删除轮播项
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool DeleteCarouselItem(Guid id);

        /// <summary>
        /// 加载轮播项
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        CarouselItem LoadItemById(Guid id);
    }
}