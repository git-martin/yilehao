/* 
    ======================================================================== 
        File name：        ActivityModule
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

namespace BntWeb.ContentMarkup
{
    public class ContentMarkupModule : IBntWebModule
    {
        public static readonly ContentMarkupModule Instance = new ContentMarkupModule();

        public string InnerKey => "BntWeb-ContentMarkup";
        public static string Key => Instance.InnerKey;
        public string InnerDisplayName => "内容标记";
        public static string DisplayName => Instance.InnerDisplayName;
        public string InnerArea => "ContentMarkup";
        public static string Area => Instance.InnerArea;
        public int InnerPosition => 7000;
        public static int Position => Instance.InnerPosition;
    }
}