using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Daidan.Web.Infrastructure
{
	public class RedirectAuthorizeAttribute : AuthorizeAttribute
	{
		protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
		{
			filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "SystemAdmins", action = "Unauthorized" }));
		}
	}
}