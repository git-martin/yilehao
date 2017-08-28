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

namespace BntWeb.Core.SystemUsers
{
    public class SystemUsersModule : IBntWebModule
    {

        public static readonly SystemUsersModule Instance = new SystemUsersModule();

        public string InnerKey => "Core-SystemUsers";
        public static string Key => Instance.InnerKey;
        public string InnerDisplayName => "后台用户";
        public static string DisplayName => Instance.InnerDisplayName;
        public string InnerArea => "SystemUsers";
        public static string Area => Instance.InnerArea;
        public int InnerPosition => 9800;
        public static int Position => Instance.InnerPosition;
    }
}