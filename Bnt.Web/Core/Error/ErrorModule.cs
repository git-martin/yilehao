/* 
    ======================================================================== 
        File name：        SystemUsersModule
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/5/27 8:57:07
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BntWeb.Environment;

namespace BntWeb.Core.Error
{
    public class ErrorModule : IBntWebModule
    {
        public static readonly ErrorModule Instance = new ErrorModule();

        public string InnerKey => "Core-Error";
        public static string Key => Instance.InnerKey;
        public string InnerDisplayName => "系统错误";
        public static string DisplayName => Instance.InnerDisplayName;
        public string InnerArea => "Error";
        public static string Area => Instance.InnerArea;
        public int InnerPosition => 9900;
        public static int Position => Instance.InnerPosition;
    }
}