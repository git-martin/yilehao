using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace BntWeb.Logistics.Services
{
    public interface IShippingAreaService: IDependency
    {
        /// <summary>
        /// 获取配送区域分页列表
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="expression"></param>
        /// <param name="orderByExpression"></param>
        /// <param name="isDesc"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        List<Models.ShippingArea> GetListPaged<TKey>(int pageIndex, int pageSize,
            Expression<Func<Models.ShippingArea, bool>> expression,
            Expression<Func<Models.ShippingArea, TKey>> orderByExpression, bool isDesc, out int totalCount);

        /// <summary>
        /// 根据id获取配送区域信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Models.ShippingArea GetById(Guid id);

        ///// <summary>
        ///// 根据区域id获取配送区域信息
        ///// </summary>
        ///// <param name="areaId"></param>
        ///// <returns></returns>
        //Models.ShippingArea GetByAreaId(string areaId);

        /// <summary>
        /// 根据区域id获取配送区域费用信息
        /// </summary>
        /// <param name="areaId"></param>
        /// <returns></returns>
        Models.ShippingAreaFee GetFeeByAreaId(string areaId);

        bool SaveInfo(Models.ShippingArea model, List<Models.ShippingAreaFee> feeList, bool insert = true);
        /// <summary>
        /// 编辑配送区域信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool Create(Models.ShippingArea model);

        /// <summary>
        /// 配送区域信息编辑
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool Update(Models.ShippingArea model);

        /// <summary>
        /// 根据id删除配送区域信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Delete(Guid id);

        /// <summary>
        /// 根据配送区域主键id获取区域费用列表
        /// </summary>
        /// <param name="shippingAreaId"></param>
        /// <returns></returns>
        List<Models.ShippingAreaFee> GetFeeListByShippingAreaId(Guid shippingAreaId);

        /// <summary>
        /// 根据省市id获取区域配送费用
        /// </summary>
        /// <param name="provinceId"></param>
        /// <param name="cityId"></param>
        /// <returns></returns>
        decimal GetAreaFreight(string provinceId, string cityId);

        /// <summary>
        /// 根据省市获取配送费用
        /// </summary>
        /// <param name="provinceId"></param>
        /// <param name="cityId"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        decimal GetAreaFreight(string provinceId, string cityId, decimal weight);

        /// <summary>
        /// 获取默认配送费用地区
        /// </summary>
        /// <returns></returns>
        Models.ShippingArea GetDefaultShippingArea();
        /// <summary>
        /// 不配送区域
        /// </summary>
        /// <param name="areaId"></param>
        /// <returns></returns>
        Models.ShippingAreaFee NotShippingArea(string areaId);
    }
}