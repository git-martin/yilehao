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

using System.ComponentModel.DataAnnotations;

namespace BntWeb.Core.SystemUsers.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Display(Name = "记住我?")]
        public bool RememberMe { get; set; } = true;

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; set; }

        public string ReturnUrl { get; set; }
    }
}