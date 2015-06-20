using Daidan.Web.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Daidan.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
		readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected void Application_Start()
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);
			AreaRegistration.RegisterAllAreas();

			GlobalFilters.Filters.Add(new HandleErrorAttribute());
			GlobalFilters.Filters.Add(new AuthorizeAttribute());

			ControllerBuilder.Current.SetControllerFactory(new NinjectControllerFactory());

			log4net.Config.XmlConfigurator.Configure(new FileInfo(Server.MapPath("~/Web.config")));
        }

		protected void Application_Error(object sender, EventArgs e)
		{
			Exception exc = Server.GetLastError();
			logger.Error(exc.ToString());
			logger.Error(exc.StackTrace);
			logger.Error("*******");
		}
    }
}
