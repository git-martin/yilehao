using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using BntWeb.Environment;
using BntWeb.FileSystems.Media;

namespace BntWeb.Evaluate.Services
{
    public interface IEvaluateService : IDependency
    {

        /// <summary>
        /// 保存评价
        /// </summary>
        /// <param name="evaluate"></param>
         void SaveEvaluate(Models.Evaluate evaluate);

        /// <summary>
        /// 获取评价列表
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="module"></param>
        /// <param name="sourceType"></param>
        /// <param name="totalCount"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        List<Models.Evaluate> LoadByPage(Guid sourceId, IBntWebModule module, string sourceType,
            out int totalCount, int pageIndex = 1, int pageSize = 10);

        /// <summary>
        /// 删除评价
        /// </summary>
        /// <param name="evaluateId"></param>
         void DeleteEvaluate(Guid evaluateId);

        /// <summary>
        /// 获取评价详细
        /// </summary>
        /// <param name="evaluateId"></param>
        /// <returns></returns>
         Models.Evaluate GetEvaluate(Guid evaluateId);


        List<StorageFile> GetEvaluateFile(Guid evaluateId, string fileTypeName);

        /// <summary>
        /// 添加订单评论
        /// </summary>
        /// <param name="evaluates"></param>
        /// <param name="orderId"></param>
        void CreateOrderEvaluates(List<Models.Evaluate> evaluates, Guid orderId);

        /// <summary>
        /// 订单评论回复
        /// </summary>
        /// <param name="evaluates"></param>
        /// <param name="orderId"></param>
        /// <param name="moduleKey"></param>
        void ReplayOrderEvaluates(List<Models.Evaluate> evaluates, Guid orderId,string moduleKey);

        /// <summary>
        /// 审核通过评论
        /// </summary>
        /// <param name="evaluateId"></param>
        bool ApprvoeEvaluate(List<Guid> evaluateId);
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="evaluateIds"></param>
        /// <returns></returns>
        bool DeleteEvaluates(List<Guid> evaluateIds);
    }
}