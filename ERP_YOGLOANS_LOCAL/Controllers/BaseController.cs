using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP_YOGLOANS_LOCAL.Controllers
{


    public class BaseController : Controller
    {


        protected override void OnActionExecuting(ActionExecutingContext filterContext)


        {

            if (Session["IsLoggedIn"] == null || !(bool)Session["IsLoggedIn"])
            {
                // User is not logged in, redirect to login page or return error message
                filterContext.Result = new RedirectResult("~/Smart_Login/Login");
                return;
            }

            base.OnActionExecuting(filterContext);
        }



    }
}