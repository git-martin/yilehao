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

namespace BntWeb.Mall
{
    public class MallModule : IBntWebModule
    {
        public static readonly MallModule Instance = new MallModule();

        public string InnerKey => "BntWeb-Mall";
        public static string Key => Instance.InnerKey;
        public string InnerDisplayName => "商城管理";
        public static string DisplayName => Instance.InnerDisplayName;
        public string InnerArea => "Mall";
        public static string Area => Instance.InnerArea;
        public int InnerPosition => 8100;
        public static int Position => Instance.InnerPosition;
    }
}