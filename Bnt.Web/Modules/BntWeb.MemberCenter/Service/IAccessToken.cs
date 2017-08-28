using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BntWeb.MemberCenter.Service
{
    public interface IAccessToken : IDependency
    {


        string GetAppId();

        string GetToken();


        TicketModel GetJsApiTicket(DateTime dtNow);


        int ConvertDateTimeInt(DateTime time);



    }
}