using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BntWeb.Environment.Configuration;
using BntWeb.Security;

namespace BntWeb.Home.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return Redirect("~/Index.html");
            //return View();
        }
    }
}