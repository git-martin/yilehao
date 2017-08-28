/* 
    ======================================================================== 
        File name：        IoCRegister
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/5/16 15:47:46
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using BntWeb.Environment.IoC;
using BntWeb.PaymentProcess.Models;
using BntWeb.PaymentProcess.Payments;
using BntWeb.PaymentProcess.Payments.Alipay;
using BntWeb.PaymentProcess.Payments.Balance;
using BntWeb.PaymentProcess.Payments.WeiXin;

namespace BntWeb.PaymentProcess
{
    public class AutofacRegister : IRegisterProvider
    {
        public void Register(ContainerBuilder builder)
        {
            builder.RegisterType<AlipayPayment>().Named<IPaymentDispatcher>(PaymentType.Alipay.ToString().ToLower());
            builder.RegisterType<WeiXinPayment>().Named<IPaymentDispatcher>(PaymentType.WeiXin.ToString().ToLower());
            builder.RegisterType<BalancePayment>().Named<IPaymentDispatcher>(PaymentType.Balance.ToString().ToLower());


            builder.RegisterType<AlipayWithdrawal>().Named<IWithdrawalDispatcher>(PaymentType.Alipay.ToString().ToLower());
            builder.RegisterType<WeiXinWithdrawal>().Named<IWithdrawalDispatcher>(PaymentType.WeiXin.ToString().ToLower());
        }
    }
}