using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using BntWeb.Feedback.Models;
using BntWeb.FileSystems.Media;

namespace BntWeb.Feedback.Services
{
    public interface IFeedbackService : IDependency
    {
        /// <summary>
        /// 获取反馈列表
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="expression"></param>
        /// <param name="orderByExpression"></param>
        /// <param name="isDesc"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        List<Models.Feedback> GetListPaged<TKey>(int pageIndex, int pageSize,
            Expression<Func<Models.Feedback, bool>> expression,
            Expression<Func<Models.Feedback, TKey>> orderByExpression, bool isDesc, out int totalCount);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Models.Feedback GetFeedbackById(Guid id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="feedback"></param>
        /// <returns></returns>
        bool UpdateFeedback(Models.Feedback feedback);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="feedback"></param>
        /// <returns></returns>
        bool Delete(Models.Feedback feedback);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="feedback"></param>
        /// <param name="imageFiles"></param>
        void CreateFeedback(Models.Feedback feedback, List<Base64Image> imageFiles = null);

        /// <summary>
        /// 获取反馈列表
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="feedbackType"></param>
        /// <param name="sourceType"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        List<Models.Feedback> GetFeedbackList(Guid memberId, FeedbackType feedbackType, string sourceType, int pageIndex, int pageSize,
            out int totalCount);

        /// <summary>
        /// 获取反馈列表
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="souceId"></param>
        /// <param name="feedbackType"></param>
        /// <param name="sourceType"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        List<Models.Feedback> GetFeedbackListBySourceId(Guid memberId, Guid souceId, FeedbackType feedbackType, string sourceType, int pageIndex, int pageSize,
            out int totalCount);
    }
}