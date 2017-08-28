/* 
    ======================================================================== 
        File name：        MemberRegisterModel
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/5/18 8:39:36
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BntWeb.FileSystems.Media;
using BntWeb.MemberBase.Models;

namespace BntWeb.MemberCenter.ApiModels
{
    public class MemberRegisterModel
    {
        public string PhoneNumber { get; set; }

        //public string NickName { get; set; }

        public string Password { get; set; }

        public string SmsVerifyCode { get; set; }

        /// <summary>
        /// 渠道Id
        /// </summary>
        public Guid? ChannelId { get; set; }

        /// <summary>
        /// 微信openId
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 邀请码
        /// </summary>
        public string InvitationCode { get; set; }
    }
    public class MemberAvatarModel
    {
        public Guid AvatarId { get; set; }
    }
    public class MemberLoginModel
    {
        public string PhoneNumber { get; set; }

        public string Password { get; set; }

        public string MobileDevice { get; set; }

        public string OpenId { get; set; }
    }

    public class MemberLoginWithSmsModel
    {
        public string PhoneNumber { get; set; }

        public string SmsVerifyCode { get; set; }

        public string MobileDevice { get; set; }

        public string OpenId { get; set; }
    }

    public class OAuthLoginModel
    {
        public OAuthType OAuthType { get; set; }

        public string OAuthId { get; set; }

        public string MobileDevice { get; set; }
    }

    public class ResetPasswordModel
    {
        public string PhoneNumber { get; set; }

        public string Password { get; set; }

        public string SmsVerifyCode { get; set; }
    }

    public class ChangePasswordModel
    {
        public string Password { get; set; }

        public string NewPassword { get; set; }
    }

    public class ModifyMemberModel
    {
        public string NickName { get; set; }

        public SexType Sex { get; set; }

        public DateTime? Birthday { get; set; }
    }

    public class ChangePhoneNumberModel
    {
        public string PhoneNumber { get; set; }

        public string SmsVerifyCode { get; set; }

        public string NewPhoneNumber { get; set; }

        public string NewSmsVerifyCode { get; set; }
    }

    public class BoundPhoneNumberModel
    {
        public string PhoneNumber { get; set; }

        public string SmsVerifyCode { get; set; }

        public string Password { get; set; }
        /// <summary>
        /// 邀请码
        /// </summary>
        public string InvitationCode { get; set; }
    }
}