using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ServerSideSampleCode.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "This is a server-side application sample for Scan Client.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "I am happy to help you. Contact me on following email Id";
            return View();
        }
    }
}