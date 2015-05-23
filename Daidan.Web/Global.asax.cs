﻿using Daidan.Web.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Daidan.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);
			AreaRegistration.RegisterAllAreas();

			ControllerBuilder.Current.SetControllerFactory(new NinjectControllerFactory());
        }

		protected void Application_Error(object sender, EventArgs e)
		{
			Exception exc = Server.GetLastError();
		}
    }
}