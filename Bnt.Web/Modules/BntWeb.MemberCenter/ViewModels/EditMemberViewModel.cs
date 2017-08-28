/* 
    ======================================================================== 
        File name：        LoginViewModel
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
using BntWeb.MemberBase.Models;

namespace BntWeb.MemberCenter.ViewModels
{
    public class EditMemberViewModel
    {
        public string MemberId { get; set; }

        [Required]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        public string Password2 { get; set; }
        
        [Display(Name = "邮箱")]
        public string Email { get; set; }

        [Display(Name = "手机")]
        public string PhoneNumber { get; set; }

        [Display(Name = "昵称")]
        public string NickName { get; set; }

        [Display(Name = "性别")]
        public SexType Sex { get; set; }

        [Display(Name = "生日")]
        public DateTime Birthday { get; set; }

        [Display(Name = "头像路径")]
        public string Avatar { get; set; }

        /// <summary>
        /// 省级Id
        /// </summary>
        public string Member_Province { get; set; }

        /// <summary>
        /// 市级Id
        /// </summary>
        public string Member_City { get; set; }

        /// <summary>
        /// 区县级Id
        /// </summary>
        public string Member_District { get; set; }

        /// <summary>
        /// 街道/乡镇Id
        /// </summary>
        public string Member_Street { get; set; }

        /// <summary>
        /// 地区名字，每个级别之间用逗号隔开
        /// </summary>
        public string RegionName { get; set; }

        /// <summary>
        /// 详细门牌地址
        /// </summary>
        public string Address { get; set; }
    }
}