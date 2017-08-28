using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace BntWeb.Wallet.ApiModels
{
    public enum SmsRequestType
    {
        [Description("提现")]
        Withdrawals = 0
    }
}