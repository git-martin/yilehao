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

namespace BntWeb.Core.SystemSettings
{
    public class SystemSettingsModule : IBntWebModule
    {
        public static readonly SystemSettingsModule Instance = new SystemSettingsModule();

        public string InnerKey => "Core-SystemSettings";
        public static string Key => Instance.InnerKey;
        public string InnerDisplayName => "系统管理";
        public static string DisplayName => Instance.InnerDisplayName;
        public string InnerArea => "SystemSettings";
        public static string Area => Instance.InnerArea;
        public int InnerPosition => 10000;
        public static int Position => Instance.InnerPosition;
    }
}