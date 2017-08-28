/* 
    ======================================================================== 
        File name：        ConfigData
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/7/27 13:01:45
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BntWeb.Data;

namespace BntWeb.Config.Models
{
    [Table(KeyGenerator.TablePrefix + "Configs")]
    public class ConfigData
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(50)]
        public string Key { get; set; }

        [MaxLength(6000)]
        public string Value { get; set; }
    }
}