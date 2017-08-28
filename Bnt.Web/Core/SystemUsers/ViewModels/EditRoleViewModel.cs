/* 
    ======================================================================== 
        File name：        EditRoleViewModel
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
    public class EditRoleViewModel
    {
        public string RoleId { get; set; }
        
        [Display(Name = "角色名")]
        public string RoleName { get; set; }

        [Required]
        [Display(Name = "角色显示名")]
        public string DisplayName { get; set; }

        [Display(Name = "角色描述")]
        public string Description { get; set; }

        public List<string> Permissions { get; set; }
    }
}