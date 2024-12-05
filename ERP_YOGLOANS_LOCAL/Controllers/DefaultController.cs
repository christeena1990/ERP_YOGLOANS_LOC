using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP_YOGLOANS_LOCAL.Controllers
{
    public class DefaultController : Controller
    {
        // GET: Default
        public ActionResult demo_Index()
        {
            return View();
        }
        public ActionResult demo_Table()
        {
            return View();
        }


        public ActionResult demo_demo()
        {
            return View();
        }

        public ActionResult demo_menu()
        {
            var tree = Session["CheckedFilterTreeValues"];


            return View();
        }

    }
}