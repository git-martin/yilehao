/* 
    ======================================================================== 
        File name：        EditUserViewModel
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/5/18 8:39:36
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BntWeb.Core.SystemUsers.ViewModels
{
    public class EditUserViewModel
    {
        public string UserId { get; set; }

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

        [Display(Name = "角色Id")]
        public List<string> Roles { get; set; }
    }
}