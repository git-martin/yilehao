/* 
    ======================================================================== 
        File name：        EditDistrictViewModel
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/6/2 15:31:01
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BntWeb.Core.SystemSettings.ViewModels
{
    public class EditDistrictViewModel
    {
        public string Id { get; set; }

        public string ParentId { get; set; }

        public string FullName { get; set; }

        public string ShortName { get; set; }

        public decimal Lng { get; set; }

        public decimal Lat { get; set; }

        public int Sort { get; set; }

        public int EditMode { get; set; }
    }

    public enum DistrictSelectLevel
    {
        /// <summary>
        /// 省级
        /// </summary>
        Province = 1,

        /// <summary>
        /// 市级
        /// </summary>
        City = 2,

        /// <summary>
        /// 区级
        /// </summary>
        District = 3,

        /// <summary>
        /// 街道
        /// </summary>
        Street = 4
    }
}