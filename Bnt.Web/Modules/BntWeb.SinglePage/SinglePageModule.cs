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

namespace BntWeb.SinglePage
{
    public class SinglePageModule : IBntWebModule
    {
        public static readonly SinglePageModule Instance = new SinglePageModule();

        public string InnerKey => "BntWeb-SinglePage";
        public static string Key => Instance.InnerKey;
        public string InnerDisplayName => "单页管理";
        public static string DisplayName => Instance.InnerDisplayName;
        public string InnerArea => "SinglePage";
        public static string Area => Instance.InnerArea;
        public int InnerPosition => 7800;
        public static int Position => Instance.InnerPosition;
    }
}