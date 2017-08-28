/* 
    ======================================================================== 
        File name：        EditTagViewModel
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/6/21 19:45:53
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BntWeb.Tag.ViewModels
{
    public class EditTagViewModel
    {
        public Guid Id { get; set; }

        public string Content { get; set; }

        public int Sort { get; set; }
    }
}