using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ITS2013.SocialWeb.Controllers
{
    public class HomeController : FacebookController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Friends()
        {
            return View();
        }
    }
}
