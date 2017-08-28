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
    public class PartnerModel
    {
        public string MemberName { get; set; }

        public decimal Money { get; set; }

        public SimplifiedStorageFile Avatar { get; set; }
    }

    public class CommissionModel
    {

        public string Remark { get; set; }

        public DateTime CreateTime { get; set; }

        public decimal Money { get; set; }

        public string MemberName { get; set; }
    }
}