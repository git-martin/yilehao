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

namespace BntWeb.Carousel
{
    public class CarouselModule : IBntWebModule
    {

        public static readonly CarouselModule Instance = new CarouselModule();

        public string InnerKey => "BntWeb-Carousel";
        public static string Key => Instance.InnerKey;
        public string InnerDisplayName => "轮播管理";
        public static string DisplayName => Instance.InnerDisplayName;
        public string InnerArea => "Carousel";
        public static string Area => Instance.InnerArea;
        public int InnerPosition => 8900;
        public static int Position => Instance.InnerPosition;
    }
}