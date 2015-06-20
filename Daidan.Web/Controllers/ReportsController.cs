using Daidan.Domain;
using Daidan.Web.Helpers;
using Daidan.Web.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Daidan.Web.Controllers
{
	[RedirectAuthorize(Roles = "admin, systemAdmin")]
    public class ReportsController : Controller
    {
		private IDataRepository dbRepository;
		public ReportsController(IDataRepository repo)
		{
			dbRepository = repo;
		}

		public ActionResult MasterReport()
		{
			return View(DaidanControllersHelper.GetTripLookups(dbRepository));
		}

		[HttpPost]
		public ActionResult MasterReportSearch(MasterReportSearchParameters parameter)
		{
			return Json(dbRepository.MasterReportSearch(parameter).OrderBy(x => x.Date).ToList(), JsonRequestBehavior.AllowGet);
		}

		public ActionResult CustomerReport()
		{
			return View(DaidanControllersHelper.GetTripLookups(dbRepository));
		}
    }
}