/* 
    ======================================================================== 
        File name：		Activity
        Module:			
        Author：		Daniel.Wu（wujb）
        Create Time：		2016/6/17 16:26:51
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using BntWeb.Data;
using BntWeb.Environment;
using BntWeb.FileSystems.Media;

namespace BntWeb.SystemMessage.Models
{
    [Table(KeyGenerator.TablePrefix + "system_message_recievers")]
    public class SystemMessageReciever
    {
        /// <summary>
        /// 消息Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 消息ID
        /// </summary>
        public Guid MessageId { get; set; }

        /// <summary>
        /// 收信人
        /// </summary>
        public string RecieveId { get; set; }

        /// <summary>
        /// 阅读时间
        /// </summary>
        public DateTime? ReadTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public Status Status { get; set; }
    }

    public class SystemMessageUnion
    {
        public SystemMessageUnion()
        {

        }
        public SystemMessageUnion(SystemMessage message)
        {
            Id = message.Id;
            Content = message.Content;
            Category = message.Category;
            SourceId = message.SourceId;
            SourceType = message.SourceType;
            CreateTime = message.CreateTime;
        }


        /// <summary>
        /// 消息Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        public MessageCategory Category { get; set; }

        /// <summary>
        /// 来源Id
        /// </summary>
        public Guid? SourceId { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string SourceType { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 已读
        /// </summary>
        public bool HasRead { get; set; }

    }

    public enum MessageType
    {
        [Description("私有消息")]
        Personal = 0,

        [Description("公共消息")]
        Public = 1
    }

    public enum Status
    {
        [Description("已删除")]
        Deleted = -1,

        [Description("未读")]
        UnRead = 0,

        [Description("已读")]
        HasRead = 1
    }
}