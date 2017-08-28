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
    public class SystemMessageRecievers
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
        public Guid RecieveId { get; set; }

        /// <summary>
        /// 来源Id
        /// </summary>
        public Guid? SourceId { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        public string SourceType { get; set; }
        /// <summary>
        /// 模块Key
        /// </summary>
        public string ModuleKey { get; set; }

        /// <summary>
        /// 阅读状态：0-未读；1-已读；
        /// </summary>
        public ReadStatus ReadStatus { get; set; }

        /// <summary>
        /// 阅读时间
        /// </summary>
        public DateTime? ReadTime { get; set; }

        public string Remark { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public Status Status { get; set; }
    }



    public class SetReadMessages
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
        public Guid RecieveId { get; set; }

        /// <summary>
        /// 来源Id
        /// </summary>
        public Guid? SourceId { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        public string SourceType { get; set; }

        /// <summary>
        /// 阅读状态：0-未读；1-已读；
        /// </summary>
        public ReadStatus ReadStatus { get; set; }

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

    public class BackSystemMessage
    { 

        /// <summary>
        /// 消息Id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        ///内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 已读
        /// </summary>
        public int Readed { get; set; }


        public Guid? SouceId { get; set; }

        public string SourceType { get; set; }

        public string Remark { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }


        public string ModuelKey { get; set; }

        public BackSystemMessage()
        {
           
        }

    }





    public enum ReadStatus
    {
        /// <summary>
        /// 未读
        /// </summary>
        [Description("未读")]
        UnRead = 0,
        /// <summary>
        /// 已读
        /// </summary>
        [Description("已读")]
        Readed = 1
    }

    public enum IsPublic
    {
        Personal = 0,
        Public = 1
    }

    public enum Status
    {
        [Description("不正常")]
        Deleted = 0,
        [Description("正常")]
        Normal = 1
    }
}