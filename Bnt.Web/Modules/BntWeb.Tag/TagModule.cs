/* 
    ======================================================================== 
        File name：        TagModule
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/6/16 11:50:31
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BntWeb.Environment;

namespace BntWeb.Tag
{
    public class TagModule : IBntWebModule
    {
        public static readonly TagModule Instance = new TagModule();

        public string InnerKey => "BntWeb-Tag";
        public static string Key => Instance.InnerKey;
        public string InnerDisplayName => "标签管理";
        public static string DisplayName => Instance.InnerDisplayName;
        public string InnerArea => "Tag";
        public static string Area => Instance.InnerArea;
        public int InnerPosition => 7000;
        public static int Position => Instance.InnerPosition;
    }
}