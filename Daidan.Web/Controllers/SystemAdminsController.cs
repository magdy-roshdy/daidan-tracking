using Daidan.Domain;
using Daidan.Entities;
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
		public ActionResult Login(SystemAdmin loginUser)
		{
			if (!ModelState.IsValid)
			{
				return View(loginUser);
			}

			SystemAdmin admin = dbRepository.GetSystemAdminByEmail(loginUser.Email);
			if (admin != null && admin.IsActive)
			{
				bool valid = Helpers.DaidanControllersHelper.VerifyPasswordHash(loginUser.Password, admin.Password);
				if(valid)
				{
					var identity = new ClaimsIdentity(new[] {
						new Claim(ClaimTypes.Name, admin.Name),
						new Claim(ClaimTypes.Email, admin.Email),
						new Claim(ClaimTypes.Role, admin.Role)
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
    }
}