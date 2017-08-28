/* 
    ======================================================================== 
        File name：        District
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/5/31 20:02:45
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using BntWeb.Data;

namespace BntWeb.Core.SystemSettings.Models
{
    [Table(KeyGenerator.TablePrefix + "System_Districts")]
    public class District
    {
        [Key]
        public string Id { get; set; }

        public string ParentId { get; set; }

        public string FullName { get; set; }

        public string PinYin { get; set; }

        public string MergerName { get; set; }

        public string ShortName { get; set; }

        public string JianPin { get; set; }

        public string MergerShortName { get; set; }

        public string FirstChar { get; set; }

        public decimal Lng { get; set; }

        public decimal Lat { get; set; }

        public int Level { get; set; }

        public string Position { get; set; }

        public int Sort { get; set; }


    }
}