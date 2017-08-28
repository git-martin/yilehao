using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BntWeb.Data.Services;
using BntWeb.FileSystems.Media;
using BntWeb.Logging;
using BntWeb.Security;

namespace BntWeb.SinglePage.Services
{
    public class SinglePageService : ISinglePageService
    {
        private readonly ICurrencyService _currencyService;
        private readonly IStorageFileService _storageFileService;
        public ILogger Logger { get; set; }
        public SinglePageService(ICurrencyService currencyService, IStorageFileService storageFileService)
        {
            _currencyService = currencyService;
            _storageFileService = storageFileService;
        }

        public List<Models.SinglePage> GetListPaged<TKey>(int pageIndex, int pageSize, Expression<Func<Models.SinglePage, bool>> expression, Expression<Func<Models.SinglePage, TKey>> orderByExpression, bool isDesc, out int totalCount)
        {
            using (var dbContext = new SinglePageDbContext())
            {
                var query = dbContext.SinglePage.Where(expression);
                totalCount = query.Count();
                if (isDesc)
                    query = query.OrderByDescending(orderByExpression);
                else
                    query = query.OrderBy(orderByExpression);

                var list = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                return list;
            }
        }

        public bool Delete(Models.SinglePage singlePage)
        {
            throw new NotImplementedException();
        }

        public Models.SinglePage GetSinglePageById(Guid id)
        {
            return _currencyService.GetSingleById<Models.SinglePage>(id);
        }

        public Models.SinglePage GetSinglePageByKey(string key)
        {
            return _currencyService.GetSingleByConditon<Models.SinglePage>(me => me.Key == key);
        }

        public bool UpdateSinglePage(Models.SinglePage singlePage)
        {
            var result = _currencyService.Update<Models.SinglePage>(singlePage);
            if (result)
                Logger.Operation($"处理反馈:{singlePage.Id}", SinglePageModule.Instance, SecurityLevel.Normal);
            return result;
        }
    }
}