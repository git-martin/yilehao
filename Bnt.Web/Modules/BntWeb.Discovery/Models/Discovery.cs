using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BntWeb.Data;

namespace BntWeb.Discovery.Models
{
    /// <summary>
    /// 实体：Discovery
    /// </summary>
    [Table(KeyGenerator.TablePrefix + "Discoveries")]
    public class Discovery
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [MaxLength(50)]
        public string Title { get; set; }

        /// <summary>
        /// 作者
        /// </summary>
        [MaxLength(50)]
        public string Author { get; set; }

        /// <summary>
        /// 简介
        /// </summary>
        [MaxLength(250)]
        public string Blurb { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        [MaxLength(125)]
        public string Source { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 读取数
        /// </summary>
        public int ReadNum { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 最后一次更新时间
        /// </summary>
        public DateTime LastUpdateTime { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        public string CreateUserId { get; set; }

        /// <summary>
        /// 创建人名称
        /// </summary>
        [MaxLength(50)]
        public string CreateName { get; set; }

        /// <summary>
        /// 文章状态：1－正常，0－其他
        /// </summary>
        public DiscoveryStatus Status { get; set; }

    }

    public enum DiscoveryStatus
    {
        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        Normal = 1,
        /// <summary>
        /// 其他
        /// </summary>
        [Description("其他")]
        Other = 0
    }

}
