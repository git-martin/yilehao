using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace BntWeb.SinglePage.Services
{
    public interface ISinglePageService : IDependency
    {
        List<Models.SinglePage> GetListPaged<TKey>(int pageIndex, int pageSize,
            Expression<Func<Models.SinglePage, bool>> expression,
            Expression<Func<Models.SinglePage, TKey>> orderByExpression, bool isDesc, out int totalCount);
        bool Delete(Models.SinglePage singlePage);

        Models.SinglePage GetSinglePageById(Guid id);

        Models.SinglePage GetSinglePageByKey(string key);
        bool UpdateSinglePage(Models.SinglePage singlePage);
    }
}