/* 
    ======================================================================== 
        File name：        PaymentService
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/7/15 11:46:19
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BntWeb.Caching;
using BntWeb.Data.Services;
using BntWeb.Logging;
using BntWeb.PaymentProcess.Models;

namespace BntWeb.PaymentProcess.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ICurrencyService _currencyService;
        private readonly ICacheManager _cacheManager;
        private readonly ISignals _signals;

        public PaymentService(ICurrencyService currencyService, ICacheManager cacheManager, ISignals signals)
        {
            _currencyService = currencyService;
            _cacheManager = cacheManager;
            _signals = signals;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public Payment LoadPayment(Guid id)
        {
            return _cacheManager.Get($"payment_{id}", ctx =>
            {
                ctx.Monitor(_signals.When($"payment_{id}_changed"));

                return _currencyService.GetSingleById<Payment>(id);
            });
        }

        public Payment LoadPayment(string code)
        {
            return _cacheManager.Get($"payment_{code}", ctx =>
            {
                var payment =
                    _currencyService.GetSingleByConditon<Payment>(
                        p => p.Code.Equals(code, StringComparison.OrdinalIgnoreCase));

                ctx.Monitor(_signals.When($"payment_{payment.Id}_changed"));

                return payment;
            });
        }

        public bool SavePayment(Payment payment)
        {
            _currencyService.Update(payment);
            _signals.Trigger($"payment_{payment.Id}_changed");

            return true;
        }
    }
}