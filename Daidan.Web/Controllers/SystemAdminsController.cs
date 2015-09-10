using Daidan.Domain;
using Daidan.Entities;
using Daidan.Web.Infrastructure;
using Daidan.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace Daidan.Web.Controllers
{
    public class SystemAdminsController : Controller
    {
		private IDataRepository dbRepository;
		public SystemAdminsController(IDataRepository repo)
		{
			dbRepository = repo;
		}

		[AllowAnonymous]
        public ActionResult Login()
		{
			return View();
		}

		[AllowAnonymous]
		[HttpPost]
		public ActionResult Login(LoginViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			SystemAdmin admin = dbRepository.GetSystemAdminByEmail(model.Email);
			if (admin != null && admin.IsActive)
			{
				bool valid = Helpers.DaidanControllersHelper.VerifyPasswordHash(model.Password, admin.Password);
				if(valid)
				{
					var identity = new ClaimsIdentity(new[] {
						new Claim(ClaimTypes.Name, admin.Email),
						new Claim(ClaimTypes.Email, admin.Email),
						new Claim(ClaimTypes.GivenName, admin.Name),
						new Claim(ClaimTypes.Role, admin.Role),
						new Claim(ClaimTypes.Sid, admin.Id.ToString())
					}, "Daidan-Trips-Cookie");

					var owinContext = Request.GetOwinContext();
					var authManager = owinContext.Authentication;
					authManager.SignIn(identity);

					return Redirect(GetRedirectUrl(string.Empty));
				}
			}
			

			ModelState.AddModelError("", "Invalid email or password");
			return View();
		}

		private string GetRedirectUrl(string returnUrl)
		{
			if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
			{
				return Url.Action("Add", "Trips");
			}

			return returnUrl;
		}

		public ActionResult Logout()
		{
			var owinContext = Request.GetOwinContext();
			var authManager = owinContext.Authentication;

			authManager.SignOut("Daidan-Trips-Cookie");
			return RedirectToAction("Add", "Trips");
		}

		[RedirectAuthorize]
		public ActionResult Unauthorized()
		{
			return View();
		}
    }
}