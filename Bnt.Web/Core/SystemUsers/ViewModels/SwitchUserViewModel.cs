/* 
    ======================================================================== 
        File name：        SwitchUserViewModel
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
    public class SwitchUserViewModel
    {
        public string UserId { get; set; }

        public bool Enabled { get; set; }
    }
}