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
    public class TransferViewModel
    {
        public List<Guid> ApplyIds { get; set; }

        public string PaymentCode { get; set; }
    }

    public class PayViewModel
    {
        public string PaymentCode { get; set; }

        public Guid OrderId { get; set; }

        public int UseBalance { get; set; }
    }

    public class SwitchViewModel
    {
        public Guid Id { get; set; }

        public bool Enabled { get; set; }
    }
}