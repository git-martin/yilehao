/* 
    ======================================================================== 
        File name：        SmsRequestModel
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/6/15 11:05:22
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace BntWeb.MemberCenter.ApiModels
{
    public class SmsRequestModel
    {
        public string PhoneNumber { get; set; }

        public SmsRequestType RequestType { get; set; }
    }

    public enum SmsRequestType
    {
        [Description("注册")]
        Register = 0,

        [Description("找回密码")]
        FindPassword = 1,

        [Description("修改手机")]
        ChangePhoneNumber = 2,

        [Description("验证码登录")]
        Login = 3,

        [Description("绑定手机")]
        BoundPhoneNumber = 4
    }
}