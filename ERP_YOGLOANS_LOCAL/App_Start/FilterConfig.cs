﻿using System.Web;
using System.Web.Mvc;

namespace ERP_YOGLOANS_LOCAL
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
