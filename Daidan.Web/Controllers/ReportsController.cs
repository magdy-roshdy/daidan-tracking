using Daidan.Domain;
using Daidan.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Daidan.Web.Controllers
{
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
			return Json(dbRepository.MasterReportSearch(parameter), JsonRequestBehavior.AllowGet);
		}

		public ActionResult CustomerReport()
		{
			return View(DaidanControllersHelper.GetTripLookups(dbRepository));
		}
    }
}