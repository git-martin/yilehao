/* 
    ======================================================================== 
        File name：        UserOAuth
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/8/28 15:56:12
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

namespace BntWeb.MemberBase.Models
{
    [Table(KeyGenerator.TablePrefix + "User_OAuth")]
    public class UserOAuth
    {
        /// <summary>
        /// 消息Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 会员Id
        /// </summary>
        public string MemberId { get; set; }

        public OAuthType OAuthType { get; set; }

        public string OAuthId { get; set; }

    }

    public enum OAuthType
    {
        /// <summary>
        /// 微博
        /// </summary>
        [Description("微博")]
        WeiBo = 1,

        /// <summary>
        /// 微信
        /// </summary>
        [Description("微信")]
        WeiXin = 2,

        /// <summary>
        /// QQ
        /// </summary>
        [Description("QQ")]
        QQ = 3
    }
}