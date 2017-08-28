using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using BntWeb.Data;
using BntWeb.FileSystems.Media;

namespace BntWeb.OrderProcess.Models
{
    [Table(KeyGenerator.TablePrefix + "View_Evaluate")]
    public class EvaluateView
    {

        /// <summary>
        /// 主键
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 评分
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// 评价人Id
        /// </summary>
        public string MemberId { get; set; }

        /// <summary>
        /// 评价人名称
        /// </summary>
        public string MemberName { get; set; }

        /// <summary>
        /// 关联数据Id
        /// </summary>
        public Guid SourceId { get; set; }

        /// <summary>
        /// 扩展数据Id
        /// </summary>
        public Guid ExtentsionId { get; set; }

        /// <summary>
        /// 是否匿名
        /// </summary>
        public bool IsAnonymity { get; set; }

        /// <summary>
        /// 未使用，使用时请建立枚举
        /// </summary>
        public EnumEvaluateStatus Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 回复内容
        /// </summary>
        [MaxLength(2000)]
        public string ReplyContent { get; set; }

        /// <summary>
        /// 回复人Id
        /// </summary>
        [MaxLength(36)]
        public string ReplyUserId { get; set; }

        /// <summary>
        /// 回复人名字
        /// </summary>
        public string ReplyUserName { get; set; }

        /// <summary>
        /// 回复时间
        /// </summary>
        public DateTime? ReplyTime { get; set; }

        public virtual List<StorageFile> Files { get; set; }

        public Guid OrderId { set; get; }

        public Guid GoodsId { set; get; }

        public string GoodsName { set; get; }

        public string GoodsAttribute { set; get; }

        public string OrderNo { set; get; }

        /// <summary>
        /// 文件id
        /// </summary>
        [NotMapped]
        public List<Guid> FilesId { get; set; }
    }

    public enum EnumEvaluateStatus
    {
        /// <summary>
        /// 禁用
        /// </summary>
        [Description("禁用")]
        Disable = 0,
        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        Enabled = 1
    }
}