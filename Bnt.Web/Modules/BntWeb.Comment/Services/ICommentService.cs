/* 
    ======================================================================== 
        File name：        ICommentService
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/6/17 15:35:14
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BntWeb.Environment;
using BntWeb.FileSystems.Media;

namespace BntWeb.Comment.Services
{
    public interface ICommentService : IDependency
    {
        /// <summary>
        /// 保存评论
        /// </summary>
        /// <param name="comment">评论信息</param>
        void SaveComment(Models.Comment comment);

        /// <summary>
        /// 分页加载评论数据
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="module"></param>
        /// <param name="sourceType"></param>
        /// <param name="totalCount"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        List<Models.Comment> LoadByPage(Guid sourceId, IBntWebModule module, string sourceType, out int totalCount, int pageIndex = 1, int pageSize = 10);

        /// <summary>
        /// 删除评论及其子评论，包括外链图片
        /// </summary>
        /// <param name="commentId"></param>
        void DeleteComment(Guid commentId);

        /// <summary>
        /// 根据内容Id删除所有评论
        /// </summary>
        /// <param name="sourceId"></param>
        void DeleteCommentBySourceId(Guid sourceId);

        /// <summary>
        /// 获取单条评论，不包含子评论
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        Models.Comment GetComment(Guid commentId);
    }
}