using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BntWeb.Data;
using BntWeb.FileSystems.Media;
using BntWeb.Security.Identity;

namespace BntWeb.MemberBase.Models
{
    [Table(KeyGenerator.TablePrefix + "View_Members_Full")]
    public class MemberFull
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public string PasswordHash { get; set; }

        public string SecurityStamp { get; set; }

        public string PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public bool TwoFactorEnabled { get; set; }

        public DateTime? LockoutEndDateUtc { get; set; }

        public bool LockoutEnabled { get; set; }

        public int AccessFailedCount { get; set; }

        public UserType UserType { get; set; }

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public string MobileDevice { get; set; }

        public string DynamicToken { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
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
        /// 会员分类
        /// </summary>
        public MemberType MemberType { get; set; }

        /// <summary>
        /// 省级Id
        /// </summary>
        public string Province { get; set; }

        /// <summary>
        /// 市级Id
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// 区县级Id
        /// </summary>
        public string District { get; set; }

        /// <summary>
        /// 街道/乡镇Id
        /// </summary>
        public string Street { get; set; }

        /// <summary>
        /// 地区名字，每个级别之间用逗号隔开
        /// </summary>
        public string RegionName { get; set; }

        /// <summary>
        /// 详细门牌地址
        /// </summary>
        public string Address { get; set; }
        
        /// <summary>
        /// 消费总金额
        /// </summary>
        public decimal BuyMoney { set; get; }
        /// <summary>
        /// 积分总数
        /// </summary>
        public decimal Integral { set; get; }

        /// <summary>
        /// 订单总数
        /// </summary>
        public int OrderCount { set; get; }
        

        public string InvitationCode { set; get; }

    }
}
