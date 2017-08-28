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

namespace BntWeb.SystemMessage.ViewModels
{
    public class SystemMessageViewModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public int MemberType { set; get; }

        public string UserName { set; get; }

        public int SentType { set; get; }

    }
}