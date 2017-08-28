/* 
    ======================================================================== 
        File name：        SystemUser
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/5/11 16:31:21
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BntWeb.Data;
using BntWeb.FileSystems.Media;
using BntWeb.Security.Identity;

namespace BntWeb.MemberBase.Models
{
    [Table(KeyGenerator.TablePrefix + "Members")]
    public class MemberExtension
    {
        [Key]
        [MaxLength(128)]
        public string Id { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [MaxLength(50)]
        public string NickName { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public SexType Sex { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 头像路径
        /// </summary>
        [NotMapped]
        public StorageFile Avatar { get; set; }

        /// <summary>
        /// 会员类型
        /// </summary>
        public MemberType MemberType { get; set; }

        /// <summary>
        /// 省级Id
        /// </summary>
        [MaxLength(10)]
        public string Province { get; set; }

        /// <summary>
        /// 市级Id
        /// </summary>
        [MaxLength(10)]
        public string City { get; set; }

        /// <summary>
        /// 区县级Id
        /// </summary>
        [MaxLength(10)]
        public string District { get; set; }

        /// <summary>
        /// 街道/乡镇Id
        /// </summary>
        [MaxLength(10)]
        public string Street { get; set; }

        /// <summary>
        /// 地区名字，每个级别之间用逗号隔开
        /// </summary>
        [MaxLength(100)]
        public string RegionName { get; set; }

        /// <summary>
        /// 详细门牌地址
        /// </summary>
        [MaxLength(200)]
        public string Address { get; set; }

        [MaxLength(6000)]
        public string Config { get; set; }

        /// <summary>
        /// 渠道Id
        /// </summary>
        public Guid? ChannelId { get; set; }

        /// <summary>
        /// 引荐人Id
        /// </summary>
        public string ReferrerId { get; set; }

        /// <summary>
        /// 所有上级id集合，“,”隔开
        /// </summary>
        public string ParentIds { get; set; }

        /// <summary>
        /// 邀请码
        /// </summary>
        public string InvitationCode { get; set; }

        //下面继续添加其他扩展字段，同时要补充到Member实体中
    }

    /// <summary>
    /// 会员性别
    /// </summary>
    public enum SexType
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Description("未知")]
        UnKonw = 0,
        /// <summary>
        /// 未知
        /// </summary>
        [Description("男")]
        Male = 1,
        /// <summary>
        /// 未知
        /// </summary>
        [Description("女")]
        Female = 2
    }

    /// <summary>
    /// 会员类型
    /// </summary>
    public enum MemberType
    {
        [Description("普通会员")]
        General = 0,

        [Description("合伙人")]
        Partner = 1
    }
}