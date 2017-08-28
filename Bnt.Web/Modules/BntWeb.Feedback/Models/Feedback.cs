using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using BntWeb.Data;
using System.ComponentModel;
using BntWeb.FileSystems.Media;

namespace BntWeb.Feedback.Models
{
    [Table(KeyGenerator.TablePrefix + "Feedbacks")]
    public class Feedback
    {
        /// <summary>
        /// 活动Id
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 反馈类型
        /// </summary>
        public FeedbackType FeedbackType { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [MaxLength(1000)]
        public string Content { get; set; }

        /// <summary>
        /// 联系方式
        /// </summary>
        [MaxLength(100)]
        public string Contact { get; set; }

        /// <summary>
        /// 关联主信息Id
        /// </summary>
        public Guid? SourceId { get; set; }

        /// <summary>
        /// 关联模块Key
        /// </summary>
        [MaxLength(50)]
        public string ModuleKey { get; set; }

        /// <summary>
        /// 关联模块名称
        /// </summary>
        [MaxLength(50)]
        public string ModuleName { get; set; }

        /// <summary>
        /// 关联数据类型，可空
        /// </summary>
        [MaxLength(50)]
        public string SourceType { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public Guid MemberId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 处理人
        /// </summary>
        public Guid? ProcesseUserId { get; set; }

        /// <summary>
        /// 处理时间
        /// </summary>
        public DateTime? ProcesseTime { get; set; }

        /// <summary>
        /// 处理意见
        /// </summary>
        [MaxLength(200)]
        public string ProcesseRemark { get; set; }

        /// <summary>
        /// 处理状态：0-未处理 1-已处理
        /// </summary>
        public ProcesseStatus ProcesseStatus { get; set; }

        /// <summary>
        ///  状态： -1-删除 1-正常
        /// </summary>
        public FeedbackStatus Status { get; set; }

        [NotMapped]
        public List<StorageFile> Images { get; internal set; }

    }

    public enum ProcesseStatus
    {
        /// <summary>
        /// 未处理
        /// </summary>
        [Description("未处理")]
        Untreated = 0,
        /// <summary>
        /// 已处理
        /// </summary>
        [Description("已处理")]
        Processed = 1
    }

    public enum FeedbackStatus
    {
        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        Normal = 1,
        /// <summary>
        /// 已删除
        /// </summary>
        [Description("已删除")]
        Delete = -1
    }

    /// <summary>
    /// 反馈类型
    /// </summary>
    public enum FeedbackType
    {
        /// <summary>
        /// 意见
        /// </summary>
        [Description("意见")]
        Feedback = 1,
        /// <summary>
        /// 留言
        /// </summary>
        [Description("留言")]
        Message = 2,
        /// <summary>
        /// 投诉
        /// </summary>
        [Description("投诉")]
        Complaint = 3

    }
}