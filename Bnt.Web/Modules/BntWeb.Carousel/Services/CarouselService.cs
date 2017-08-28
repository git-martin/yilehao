/* 
    ======================================================================== 
        File name：        CarouselService
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/6/23 9:24:03
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BntWeb.Carousel.Models;
using BntWeb.Data.Services;
using BntWeb.FileSystems.Media;
using BntWeb.Logging;

namespace BntWeb.Carousel.Services
{
    public class CarouselService : ICarouselService
    {
        private readonly ICurrencyService _currencyService;
        private readonly IStorageFileService _storageFileService;

        public CarouselService(ICurrencyService currencyService, IStorageFileService storageFileService)
        {
            _currencyService = currencyService;
            _storageFileService = storageFileService;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public List<CarouselGroup> LoadAllGroups()
        {
            return _currencyService.GetAll<CarouselGroup>(new OrderModelField { IsDesc = false, PropertyName = "Key" });
        }

        public List<CarouselItem> LoadItemsByGroupId(Guid id)
        {
            var carousels = _currencyService.GetList<CarouselItem>(c => c.GroupId.Equals(id), new OrderModelField { IsDesc = true, PropertyName = "Sort" });

            foreach (var carousel in carousels)
            {
                carousel.CoverImage = LoadCoverImage(carousel.Id);
            }

            return carousels;
        }

        public List<CarouselItem> LoadItemsByGroupKey(string key)
        {
            using (var dbContext = new CarouselDbContext())
            {
                var items = from c in dbContext.CarouselItems
                            join g in dbContext.CarouselGroups
                                on c.GroupId equals g.Id
                            where g.Key.Equals(key, StringComparison.OrdinalIgnoreCase)
                            orderby c.Sort descending
                            select c;

                var carousels = items.ToList();
                foreach (var carousel in carousels)
                {
                    carousel.CoverImage = LoadCoverImage(carousel.Id);
                }

                return carousels;
            }
        }

        public bool SaveCarouselItem(CarouselItem carouselItem)
        {
            try
            {

                if (_currencyService.GetSingleById<CarouselItem>(carouselItem.Id) != null)
                {
                    if (_currencyService.Update(carouselItem))
                    {
                        _storageFileService.ReplaceFile(carouselItem.Id, CarouselModule.Key, CarouselModule.DisplayName, carouselItem.CoverImage.Id, "CoverImage");
                        return true;
                    }
                    return false;
                }
                else
                {
                    if (_currencyService.Create(carouselItem))
                    {
                        _storageFileService.AssociateFile(carouselItem.Id, CarouselModule.Key, CarouselModule.DisplayName, carouselItem.CoverImage.Id, "CoverImage");
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "保存轮播出错了");
                return false;
            }
        }

        public bool DeleteCarouselItem(Guid id)
        {
            try
            {
                if (_currencyService.DeleteByConditon<CarouselItem>(c => c.Id == id) > 0)
                {
                    return _storageFileService.DisassociateFile(id, CarouselModule.Key, "CoverImage");
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "删除轮播出错了");
                return false;
            }
        }

        public CarouselItem LoadItemById(Guid id)
        {
            var carousel = _currencyService.GetSingleById<CarouselItem>(id);
            carousel.CoverImage = LoadCoverImage(carousel.Id);
            return carousel;
        }

        private StorageFile LoadCoverImage(Guid carouselId)
        {
            return _storageFileService.GetFiles(carouselId, CarouselModule.Key, "CoverImage").FirstOrDefault();
        }
    }
}