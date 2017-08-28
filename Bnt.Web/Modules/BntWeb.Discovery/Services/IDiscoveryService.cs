using System;
using System.Collections.Generic;
using BntWeb.Discovery.Models;

namespace BntWeb.Discovery.Services
{
    public interface IDiscoveryService : IDependency
    {
        /// <summary>
        /// 获取发现列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        List<Models.Discovery> GetDiscoveryByPage(int pageIndex, int pageSize, out int totalCount);

        /// <summary>
        /// 发现文章编辑
        /// </summary>
        /// <param name="model"></param>
        /// <param name="goodsIds"></param>
        Guid SaveDiscovery(Models.Discovery model, List<Guid> goodsIds);

        /// <summary>
        /// 获取发现文章关联商品
        /// </summary>
        /// <param name="discoveryId"></param>
        /// <returns></returns>
        List<DiscoveryGoodsRelation> GetDiscoveryRelationGoods(Guid discoveryId);

        /// <summary>
        /// 删除发现文章及关联商品
        /// </summary>
        /// <param name="id"></param>
        void DeleteDiscovery(Guid id);
    }
}
