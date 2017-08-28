using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using BntWeb.Logistics.Models;

namespace BntWeb.Logistics.Services
{
    public interface IShippingService : IDependency
    {
        /// <summary>
        /// 获取配送方式列表
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="expression"></param>
        /// <param name="orderByExpression"></param>
        /// <param name="isDesc"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        List<Models.Shipping> GetListPaged<TKey>(int pageIndex, int pageSize,
            Expression<Func<Models.Shipping, bool>> expression,
            Expression<Func<Models.Shipping, TKey>> orderByExpression, bool isDesc, out int totalCount);

        /// <summary>
        /// 编辑配送方式
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool UpdateShipping(Shipping model);

        /// <summary>
        /// 设置默认配送方式
        /// </summary>
        /// <param name="shippingId"></param>
        /// <returns></returns>
        void SetDefault(Guid shippingId);

        List<Shipping> GetList();
    }
}