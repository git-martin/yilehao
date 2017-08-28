using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BntWeb.Core.Error.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult AuthorizedFailed()
        {
            return View();
        }
        public ActionResult NotAuthorized()
        {
            return View();
        }
    }
}