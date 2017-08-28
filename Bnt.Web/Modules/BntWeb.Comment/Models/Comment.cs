/* 
    ======================================================================== 
        File name：        Comment
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/6/17 13:45:34
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using BntWeb.Data;
using BntWeb.FileSystems.Media;
using BntWeb.MemberBase.Models;

namespace BntWeb.Comment.Models
{
    [Table(KeyGenerator.TablePrefix + "Comments")]
    public class Comment
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 评论内容
        /// </summary>
        [MaxLength(1000)]
        public string Content { get; set; }

        /// <summary>
        /// 评分
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// 评论人编号
        /// </summary>
        [MaxLength(36)]
        public string MemberId { get; set; }

        /// <summary>
        /// 评论编号
        /// </summary>
        [MaxLength(50)]
        public string MemberName { get; set; }

        [NotMapped]
        public string MemberPhone { get; set; }

        /// <summary>
        /// 父级评论Id
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 关联主信息Id
        /// </summary>
        public Guid SourceId { get; set; }

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
        /// 匿名评论
        /// </summary>
        public bool IsAnonymity { get; set; }

        /// <summary>
        /// 评论时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 评论状态
        /// </summary>
        public CommentStatus Status { get; set; }

        public virtual List<Comment> ChildComments { get; set; }

        public virtual List<StorageFile> Files { get; set; }

        public SimplifiedComment Simplified()
        {
            return new SimplifiedComment
            {
                Id = Id,
                Content = Content,
                Score = Score,
                MemberId = MemberId,
                MemberName = MemberName,
                ParentId = ParentId,
                IsAnonymity = IsAnonymity,
                CreateTime = CreateTime,
                ChildComments = ChildComments?.Select(c => c.Simplified()).ToList() ?? new List<SimplifiedComment>(),
                Files = Files?.Select(f => f.Simplified()).ToList() ?? new List<SimplifiedStorageFile>()
            };
        }
    }

    public class SimplifiedComment
    {
        public Guid Id { get; set; }

        /// <summary>
        /// 评论内容
        /// </summary>
        [MaxLength(1000)]
        public string Content { get; set; }

        /// <summary>
        /// 评分
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// 评论人编号
        /// </summary>
        [MaxLength(36)]
        public string MemberId { get; set; }

        /// <summary>
        /// 评论编号
        /// </summary>
        [MaxLength(50)]
        public string MemberName { get; set; }

        /// <summary>
        /// 父级评论Id
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 匿名评论
        /// </summary>
        public bool IsAnonymity { get; set; }

        /// <summary>
        /// 评论时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        public virtual List<SimplifiedComment> ChildComments { get; set; }

        public virtual List<SimplifiedStorageFile> Files { get; set; }
    }

    public enum CommentStatus
    {
        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        Normal = 1,

        /// <summary>
        /// 未审核
        /// </summary>
        [Description("未审核")]
        NotAudited = 0
    }
}