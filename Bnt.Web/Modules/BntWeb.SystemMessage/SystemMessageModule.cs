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

namespace BntWeb.SystemMessage
{
    public class SystemMessageModule : IBntWebModule
    {
        public static readonly SystemMessageModule Instance = new SystemMessageModule();

        public string InnerKey => "BntWeb-SystemMessage";
        public static string Key => Instance.InnerKey;
        public string InnerDisplayName => "消息中心";
        public static string DisplayName => Instance.InnerDisplayName;
        public string InnerArea => "SystemMessage";
        public static string Area => Instance.InnerArea;
        public int InnerPosition => 9100;
        public static int Position => Instance.InnerPosition;
    }
}