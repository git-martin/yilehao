/* 
    ======================================================================== 
        File name：        ConfigPaymentViewModel
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/7/13 14:33:16
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BntWeb.PaymentProcess.ViewModels
{
    public class ConfigPaymentViewModel
    {
        public Guid Id { get; set; }

        public bool Enabled { get; set; }
    }
}