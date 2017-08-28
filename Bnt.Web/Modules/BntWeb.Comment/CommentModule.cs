/* 
    ======================================================================== 
        File name：        CommentModule
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/6/16 12:39:38
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BntWeb.Environment;

namespace BntWeb.Comment
{
    public class CommentModule : IBntWebModule
    {
        public static readonly CommentModule Instance = new CommentModule();

        public string InnerKey => "BntWeb-Comment";
        public static string Key => Instance.InnerKey;
        public string InnerDisplayName => "评论";
        public static string DisplayName => Instance.InnerDisplayName;
        public string InnerArea => "Comment";
        public static string Area => Instance.InnerArea;
        public int InnerPosition => 9600;
        public static int Position => Instance.InnerPosition;
    }
}