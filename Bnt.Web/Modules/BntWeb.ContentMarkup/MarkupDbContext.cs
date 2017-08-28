/* 
    ======================================================================== 
        File name：        MarkupDbContext
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/6/22 14:14:58
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using BntWeb.ContentMarkup.Models;
using BntWeb.Data;
using BntWeb.FileSystems.Media;
using BntWeb.MemberBase.Models;

namespace BntWeb.ContentMarkup
{
    public class MarkupDbContext : BaseDbContext
    {
        public DbSet<Markup> Markups { get; set; }

        public DbSet<Member> Members { get; set; }
    }
}