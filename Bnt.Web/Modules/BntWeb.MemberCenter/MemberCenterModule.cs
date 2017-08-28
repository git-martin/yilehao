/* 
    ======================================================================== 
        File name：        MemberCenterModule
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/6/7 15:46:55
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BntWeb.Environment;

namespace BntWeb.MemberCenter
{
    public class MemberCenterModule : IBntWebModule
    {

        public static readonly MemberCenterModule Instance = new MemberCenterModule();

        public string InnerKey => "BntWeb-MemberCenter";
        public static string Key => Instance.InnerKey;
        public string InnerDisplayName => "会员管理";
        public static string DisplayName => Instance.InnerDisplayName;
        public string InnerArea => "MemberCenter";
        public static string Area => Instance.InnerArea;
        public int InnerPosition => 9700;
        public static int Position => Instance.InnerPosition;
    }
}