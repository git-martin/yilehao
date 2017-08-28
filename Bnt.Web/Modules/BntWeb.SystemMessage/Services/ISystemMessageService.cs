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
using BntWeb.FileSystems.Media;
using BntWeb.MemberBase.Models;
using BntWeb.SystemMessage.Models;

namespace BntWeb.SystemMessage.Services
{
    public interface ISystemMessageService : IDependency
    {
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="title">消息标题</param>
        /// <param name="content">消息内容</param>
        /// <param name="recieveId">接收人，如果为Null则发给发所有人</param>
        /// <param name="sourceId"></param>
        /// <param name="sourceType"></param>
        /// <param name="moduleKey"></param>
        /// <param name="category">消息分组</param>
        /// <returns></returns>
        bool CreateSystemMessage(string title, string content, string recieveId, Guid? sourceId, string sourceType = "", string moduleKey = "", MessageCategory category = MessageCategory.System);

        /// <summary>
        /// 发送推送消息
        /// </summary>
        /// <param name="title">消息标题</param>
        /// <param name="content">消息内容</param>
        /// <param name="pushContent">推送内容</param>
        /// <param name="pushParas">推送参数</param>
        /// <param name="recieveId">接收人，如果为Null则发给发所有人</param>
        /// <param name="sourceId"></param>
        /// <param name="sourceType"></param>
        /// <param name="moduelKey"></param>
        /// <param name="category">消息分组</param>
        /// <returns></returns>
        bool CreatePushSystemMessage(string title, string content, string pushContent, string recieveId, Guid? sourceId, Dictionary<string, string> pushParas = null, string sourceType = "", string moduelKey = "", MessageCategory category = MessageCategory.System);

        /// <summary>
        /// 获取分组最新一条消息，并out未读条数
        /// </summary>
        /// <param name="category">分组</param>
        /// <param name="recieveId">消息接收人</param>
        /// <param name="unreadCount">未读条数</param>
        /// <returns></returns>
        SystemMessageUnion GetFirstMessage(MessageCategory category, string recieveId, out int unreadCount);

        /// <summary>
        /// 分页加载会员不同组的消息
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="category">消息分组</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        IEnumerable<SystemMessageUnion> LoadSystemMessageByPage(MessageCategory category, string memberId, int pageIndex, int pageSize, out int totalCount);

        /// <summary>
        /// 设置单条信息已读
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="messageId"></param>
        /// <returns></returns>
        bool SetRead(string memberId, Guid messageId);

        /// <summary>
        /// 设置一组信息已读
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        bool SetRead(string memberId, MessageCategory category);


        /// <summary>
        /// 删除系统消息
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="messageId"></param>
        /// <returns></returns>
        bool Delete(string memberId, Guid messageId);

        /// <summary>
        /// 后台使用
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        IEnumerable<Models.SystemMessage> GetSystemMessageListByPage(int pageIndex, int pageSize, out int totalCount);
    }
}