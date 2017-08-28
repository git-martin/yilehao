using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.Logging;
using BntWeb.Security;

namespace BntWeb.Logistics.Services
{
    public class ShippingAreaService : IShippingAreaService
    {
        private readonly ICurrencyService _currencyService;
        public ILogger Logger { get; set; }
        public ShippingAreaService(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        public List<Models.ShippingArea> GetListPaged<TKey>(int pageIndex, int pageSize, Expression<Func<Models.ShippingArea, bool>> expression, Expression<Func<Models.ShippingArea, TKey>> orderByExpression, bool isDesc, out int totalCount)
        {
            using (var dbContext = new LogisticsDbContext())
            {
                var query = dbContext.ShippingAreas.Where(expression);
                totalCount = query.Count();
                query = isDesc ? query.OrderByDescending(orderByExpression) : query.OrderBy(orderByExpression);

                var list = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                return list;
            }
        }

        public Models.ShippingArea GetById(Guid id)
        {
            using (var dbContext = new LogisticsDbContext())
            {
                var model = dbContext.ShippingAreas.FirstOrDefault(g => g.Id.Equals(id));
                return model;
            }
        }

        public Models.ShippingAreaFee GetFeeByAreaId(string areaId)
        {
            using (var dbContext = new LogisticsDbContext())
            {
                var model = dbContext.ShippingAreasFees.FirstOrDefault(g => g.AreaId.Equals(areaId));
                return model;
            }
        }

        public bool SaveInfo(Models.ShippingArea model, List<Models.ShippingAreaFee> feeList, bool insert = true)
        {
            using (var dbContext = new LogisticsDbContext())
            {
                if (insert)
                {
                    model.CreateTime = DateTime.Now;
                    model.LastUpdateTime = DateTime.Now;
                    model.Status = Models.ShippingAreaStatus.Normal;
                    dbContext.ShippingAreas.Add(model);
                }
                else
                {
                    model.LastUpdateTime = DateTime.Now;
                    dbContext.ShippingAreas.Attach(model);
                    dbContext.Entry(model).State = EntityState.Modified;

                    //批量删除区域费用
                    _currencyService.DeleteByConditon<Models.ShippingAreaFee>(d => d.ShippingAreaId == model.Id);
                }


                foreach (var feeInfo in feeList)
                {
                    dbContext.ShippingAreasFees.Add(feeInfo);
                }

                var result = dbContext.SaveChanges() > 0;
                if (result)
                {
                    Logger.Operation($"编辑配送区域-{model.Name}:{model.Id}", LogisticsModule.Instance);
                }
                return result;
            }
        }

        public bool Create(Models.ShippingArea model)
        {
            using (var dbContext = new LogisticsDbContext())
            {
                model.Id = KeyGenerator.GetGuidKey();
                model.CreateTime = DateTime.Now;
                model.LastUpdateTime = DateTime.Now;
                model.Status = Models.ShippingAreaStatus.Normal;
                dbContext.ShippingAreas.Add(model);
                var result = dbContext.SaveChanges() > 0;
                if (result)
                {
                    Logger.Operation($"创建配送区域-{model.Name}:{model.Id}", LogisticsModule.Instance);
                }
                return result;
            }
        }

        public bool Update(Models.ShippingArea model)
        {
            model.LastUpdateTime = DateTime.Now;
            var result = _currencyService.Update(model);
            if (result)
                Logger.Operation($"编辑配送区域:{model.Id}", LogisticsModule.Instance);
            return result;
        }

        public bool Delete(Guid id)
        {
            //删除配送区域主表
            _currencyService.DeleteByConditon<Models.ShippingArea>(d => d.Id == id);
            //批量删除配送区域费用
            _currencyService.DeleteByConditon<Models.ShippingAreaFee>(d => d.ShippingAreaId == id);

            return true;
        }

        public List<Models.ShippingAreaFee> GetFeeListByShippingAreaId(Guid shippingAreaId)
        {
            using (var dbContext = new LogisticsDbContext())
            {
                var query =
                    dbContext.ShippingAreasFees.Where(me => me.ShippingAreaId == shippingAreaId);
                return query.ToList();
            }
        }

        public decimal GetAreaFreight(string provinceId, string cityId)
        {
            var shippingArea = GetFeeByAreaId(cityId) ?? GetFeeByAreaId(provinceId);//市
            return shippingArea?.Freight ?? 0.00M;
        }

        public Models.ShippingArea GetDefaultShippingArea()
        {
            using (var dbContext = new LogisticsDbContext())
            {
                var mod = dbContext.ShippingAreas.FirstOrDefault(g => g.IsDefualt == Models.DefaultStatus.Default);
                return mod;
            }
        }

        public decimal GetAreaFreight(string provinceId, string cityId, decimal weight)
        {
            var shippingArea = GetFeeByAreaId(cityId) ?? GetFeeByAreaId(provinceId);//市
            var defaultShipping = GetDefaultShippingArea();
            var freight = defaultShipping?.Freight ?? 0.00M;
            if (shippingArea != null)
            {
                if (weight > shippingArea.Weight)
                {
                    freight = shippingArea.Freight + Math.Ceiling((weight - shippingArea.Weight)) * shippingArea.SFreight;
                }
                else
                {
                    freight = shippingArea.Freight;
                }
            }
            else
            {
                var shipping = GetDefaultShippingArea();
                if (shipping != null)
                {
                    if (weight > shipping.Weight)
                    {
                        freight = shipping.Freight + Math.Ceiling((weight - shipping.Weight)) * shipping.SFreight;
                    }
                    else
                    {
                        freight = shipping.Freight;
                    }
                }

            }
            return freight;
        }

        public Models.ShippingAreaFee NotShippingArea(string areaId)
        {
            using (var dbContext = new LogisticsDbContext())
            {
                var mod =
                    dbContext.ShippingAreasFees.FirstOrDefault(g => g.AreaId.Equals(areaId) && g.IsDefualt == Models.DefaultStatus.NotShipping);
                return mod;
            }
        }
    }
}