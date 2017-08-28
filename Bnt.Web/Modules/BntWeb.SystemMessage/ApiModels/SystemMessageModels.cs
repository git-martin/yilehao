using System;
using BntWeb.FileSystems.Media;
using BntWeb.SystemMessage.Models;

namespace BntWeb.SystemMessage.ApiModels
{
    public class SystemMessageCategory
    {
        /// <summary>
        /// 分组标识
        /// </summary>
        public MessageCategory Category { get; set; }

        /// <summary>
        /// 分组名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 未读条数
        /// </summary>
        public int UnreadCount { get; set; }

        /// <summary>
        /// 最新一条消息
        /// </summary>
        public SystemMessageUnion FirstMessage { get; set; }
    }
}