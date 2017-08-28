/* 
    ======================================================================== 
        File name：        WxRequestHandler
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/8/26 19:08:46
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Senparc.Weixin.MP.TenPayLibV3;

namespace BntWeb.PaymentProcess.Payments.WeiXin
{
    /// <summary>
    /// 对微信接口内部的RequestHandler进行的调整
    /// </summary>
    public class WxRequestHandler : RequestHandler
    {
        public WxRequestHandler()
        {
            Parameters = new Hashtable();
        }


        public WxRequestHandler(HttpContext httpContext)
        {
            Parameters = new Hashtable();

            this.HttpContext = httpContext ?? HttpContext.Current;

        }
        protected override string GetCharset()
        {
            return Encoding.UTF8.BodyName;
        }
    }
}