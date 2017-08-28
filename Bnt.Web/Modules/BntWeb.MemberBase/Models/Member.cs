/* 
    ======================================================================== 
        File name：        Member
        Module:                
        Author：            Luce
        Create Time：    2016/6/8 16:49:09
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.Environment;
using BntWeb.FileSystems.Media;
using BntWeb.Security;
using BntWeb.Security.Identity;
using BntWeb.Utility.Extensions;
using Microsoft.AspNet.Identity;

namespace BntWeb.MemberBase.Models
{
    [Table(KeyGenerator.TablePrefix + "View_Members")]
    public class Member
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
        /// 会员类型
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
        /// 用户自定义配置Json字符串
        /// 请使用转换后的对象MemberConfig
        /// </summary>
        public string Config { get; set; }

        private MemberConfig _memberConfig;
        /// <summary>
        /// 用户自定义配置实体
        /// </summary>
        [NotMapped]
        public MemberConfig MemberConfig
        {
            get
            {
                if (_memberConfig == null)
                {
                    if (Config != null)
                    {
                        var config = Config.DeserializeJsonToObject<MemberConfig>();
                        _memberConfig = config ?? new MemberConfig();
                    }
                    else
                    {
                        _memberConfig = new MemberConfig();
                    }
                }

                return _memberConfig;
            }
        }

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

        public User GetUser(DefaultUserManager userManager = null)
        {
            if (userManager == null)
                userManager = HostConstObject.Container.Resolve<DefaultUserManager>();
            var user = userManager.FindById(Id);
            if (user == null)
            {
                user = new User
                {
                    UserType = UserType,
                    UserName = UserName,
                    Email = Email,
                    PhoneNumber = PhoneNumber,
                    LockoutEnabled = false,
                    DynamicToken = DynamicToken,
                    MobileDevice = MobileDevice
                };
            }
            else
            {
                user.UserType = UserType;
                user.UserName = UserName;
                user.Email = Email;
                user.PhoneNumber = PhoneNumber;
                user.DynamicToken = DynamicToken;
                user.MobileDevice = MobileDevice;
            }
            return user;
        }

        public MemberExtension GetMemberExtension()
        {
            var memberExtension = HostConstObject.Container.Resolve<ICurrencyService>().GetSingleById<MemberExtension>(Id);
            if (memberExtension == null)
            {
                memberExtension = new MemberExtension
                {
                    Id = Id,
                    NickName = NickName,
                    Birthday = Birthday,
                    Sex = Sex,
                    MemberType = MemberType,
                    Province = Province,
                    City = City,
                    District = District,
                    Street = Street,
                    RegionName = RegionName,
                    Address = Address,
                    Config = MemberConfig.ToJson(),
                    ChannelId = ChannelId,
                    ReferrerId = ReferrerId,
                    ParentIds = ParentIds,
                    InvitationCode = InvitationCode
                };
            }
            else
            {
                memberExtension.NickName = NickName;
                memberExtension.Birthday = Birthday;
                memberExtension.Sex = Sex;
                memberExtension.MemberType = MemberType;
                memberExtension.Province = Province;
                memberExtension.City = City;
                memberExtension.District = District;
                memberExtension.Street = Street;
                memberExtension.RegionName = RegionName;
                memberExtension.Address = Address;
                memberExtension.Config = MemberConfig.ToJson();
                memberExtension.ChannelId = ChannelId;
                memberExtension.ReferrerId = ReferrerId;
                memberExtension.ParentIds = ParentIds;
                memberExtension.InvitationCode = InvitationCode;
            }

            return memberExtension;
        }

        public SimplifiedMember Simplified()
        {
            return new SimplifiedMember
            {
                Id = Id,
                UserName = UserName,
                Email = Email,
                PhoneNumber = PhoneNumber,
                CreateTime = CreateTime,
                MobileDevice = MobileDevice,
                DynamicToken = DynamicToken,
                NickName = NickName,
                Sex = Sex,
                Birthday = Birthday,
                Avatar = Avatar?.Simplified(),
                MemberType = MemberType,
                Province = Province,
                City = City,
                District = District,
                Street = Street,
                RegionName = RegionName,
                Address = Address,
                MemberConfig = MemberConfig,
                InvitationCode = InvitationCode
            };
        }
    }

    /// <summary>
    /// 简化的会员对象，可以作为接口返回数据
    /// </summary>
    public class SimplifiedMember
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public DateTime CreateTime { get; set; }

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
        public SimplifiedStorageFile Avatar { get; set; }

        /// <summary>
        /// 会员类型
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
        /// 用户自定义配置
        /// </summary>
        public MemberConfig MemberConfig { get; set; }


        /// <summary>
        /// 邀请码
        /// </summary>
        public string InvitationCode { get; set; }
    }
}
