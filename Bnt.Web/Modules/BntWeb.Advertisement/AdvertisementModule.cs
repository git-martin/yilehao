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

namespace BntWeb.Advertisement
{
    public class AdvertisementModule : IBntWebModule
    {

        public static readonly AdvertisementModule Instance = new AdvertisementModule();

        public string InnerKey => "BntWeb-Advertisement";
        public static string Key => Instance.InnerKey;
        public string InnerDisplayName => "广告管理";
        public static string DisplayName => Instance.InnerDisplayName;
        public string InnerArea => "Advertisement";
        public static string Area => Instance.InnerArea;
        public int InnerPosition => 8900;
        public static int Position => Instance.InnerPosition;
    }
}