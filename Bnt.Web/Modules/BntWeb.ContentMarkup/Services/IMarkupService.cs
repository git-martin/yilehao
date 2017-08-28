/* 
    ======================================================================== 
        File name：        IMarkupService
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/6/22 13:47:48
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BntWeb.ContentMarkup.Models;
using BntWeb.MemberBase.Models;

namespace BntWeb.ContentMarkup.Services
{
    public interface IMarkupService : IDependency
    {
        /// <summary>
        /// 创建标记
        /// </summary>
        /// <param name="sourceId">内容Id</param>
        /// <param name="moduleKey">模块Key</param>
        /// <param name="memberId">标记会员Id</param>
        /// <param name="markupType">标记类型</param>
        /// <param name="sourceType">内容类型，自定义，可空</param>
        /// <param name="moduleName">模块名字</param>
        /// <returns></returns>
        bool CreateMarkup(Guid sourceId, string moduleKey, string memberId, MarkupType markupType, string sourceType = null, string moduleName = null);

        /// <summary>
        /// 取消标签
        /// </summary>
        /// <param name="sourceId">内容Id</param>
        /// <param name="moduleKey">模块Key</param>
        /// <param name="memberId">标记会员Id</param>
        /// <param name="markupType">标记类型</param>
        /// <returns></returns>
        bool CancelMarkup(Guid sourceId, string moduleKey, string memberId, MarkupType markupType);

        /// <summary>
        /// 清理标记
        /// </summary>
        /// <param name="moduleKey"></param>
        /// <param name="memberId"></param>
        /// <param name="markupType"></param>
        /// <returns></returns>
        bool ClearMarkup(string moduleKey, string memberId, MarkupType markupType);
        
        /// <summary>
        /// 判断是否已经标记
        /// </summary>
        /// <param name="sourceId">内容Id</param>
        /// <param name="moduleKey">模块Key</param>
        /// <param name="memberId">标记会员Id</param>
        /// <param name="markupType">标记类型</param>
        /// <returns></returns>
        bool MarkupExist(Guid sourceId, string moduleKey, string memberId, MarkupType markupType);

        /// <summary>
        /// 查看内容总标记数
        /// </summary>
        /// <param name="sourceId">内容Id</param>
        /// <param name="moduleKey">模块Key</param>
        /// <param name="markupType">标记类型</param>
        /// <returns></returns>
        int MarkupCount(Guid sourceId, string moduleKey, MarkupType markupType);

        /// <summary>
        /// 分页加载所有标记的人
        /// </summary>
        /// <param name="sourceId">内容Id</param>
        /// <param name="moduleKey">模块Key</param>
        /// <param name="markupType">标记类型</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        List<Member> LoadByPage(Guid sourceId, string moduleKey, MarkupType markupType, int pageIndex, int pageSize, out int totalCount);

    }
}