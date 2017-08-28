using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.Logging;
using BntWeb.Logistics.Models;

namespace BntWeb.Logistics.Services
{
    public class ShippingService: IShippingService
    {
        private readonly ICurrencyService _currencyService;

        public ILogger Logger { get; set; }
        public ShippingService(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        public List<Models.Shipping> GetListPaged<TKey>(int pageIndex, int pageSize, Expression<Func<Models.Shipping, bool>> expression, Expression<Func<Models.Shipping, TKey>> orderByExpression, bool isDesc, out int totalCount)
        {
            using (var dbContext = new LogisticsDbContext())
            {
                var query = dbContext.Shippings.Where(expression);
                totalCount = query.Count();
                query = isDesc ? query.OrderByDescending(orderByExpression) : query.OrderBy(orderByExpression);

                var list = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                return list;
            }
        }

        public bool UpdateShipping(Shipping model)
        {
            var result =  _currencyService.Update(model);
            return result;
        }

        public void SetDefault(Guid shippingId)
        {
            using (var dbContext = new LogisticsDbContext())
            {
                var defaultList =
                   dbContext.Shippings.Where(me => me.IsDefault||me.Id.Equals(shippingId)).ToList();
                foreach (Shipping shipping in defaultList)
                {
                    shipping.IsDefault = shipping.Id.Equals(shippingId)? true:false;
                }
                dbContext.SaveChanges();
            }
        }

        public List<Shipping> GetList()
        {
            using (var dbContext = new LogisticsDbContext())
            {
                var query =
                    dbContext.Shippings.Where(me => me.Status > 0)
                        .OrderByDescending(me => me.CreateTime);
                return query.ToList();
            }
        }
    }
}