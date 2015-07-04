using Daidan.Domain;
using Daidan.Entities;
using Daidan.Web.Helpers;
using Daidan.Web.Infrastructure;
using Daidan.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Daidan.Web.Controllers
{
    public class TripsController : Controller
    {
		private IDataRepository dbRepository;
		public TripsController(IDataRepository repo)
		{
			dbRepository = repo;
		}

		[RedirectAuthorize(Roles = "data-entry, admin, systemAdmin")]
        public ActionResult Add()
        {
			AddTripViewModel viewModel = new AddTripViewModel();
			viewModel.AddTripLookup = DaidanControllersHelper.GetTripLookups(dbRepository);

			return View(viewModel);
        }

		[HttpPost]
		[RedirectAuthorize(Roles = "data-entry, admin, systemAdmin")]
		public ActionResult GetAddedTodayTrips()
		{
			return Json(dbRepository.GetAddedTodayTrips().OrderByDescending(x => x.AddedOn).ToList());
		}

		[HttpPost]
		[RedirectAuthorize(Roles = "data-entry, admin, systemAdmin")]
		public ActionResult SaveTrip(Trip trip)
		{
			if(trip != null)
			{
				ClaimsIdentity admin = User.Identity as ClaimsIdentity;
				int id = int.Parse(admin.FindFirst(ClaimTypes.Sid).Value);

				if (trip.Id > 0)
					trip.LastModefiedBy = new SystemAdmin { Id = id };
				else
					trip.AddedBy = new SystemAdmin { Id = id };
				
				
				trip = dbRepository.SaveTrip(trip);
			}

			return Json(trip, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[RedirectAuthorize(Roles = "admin, systemAdmin")]
		public ActionResult DeletedTrip(long tripId)
		{
			dbRepository.DeleteTrip(tripId);
			return Json("", JsonRequestBehavior.AllowGet);
		}

		public ActionResult GetTrip(long tripId)
		{
			return Json(dbRepository.GetTripById(tripId), JsonRequestBehavior.AllowGet);
		}

		public ActionResult MasterSearch()
		{
			return View(DaidanControllersHelper.GetTripLookups(dbRepository));
		}

		[HttpPost]
		public ActionResult MasterSearch(MasterReportSearchParameters parameter)
		{
			return Json(dbRepository.MasterReportSearch(parameter).OrderBy(x => x.Date).ToList(), JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[RedirectAuthorize(Roles = "admin, systemAdmin")]
		public ActionResult PONumberBatchUpdate(PONumberBatchUpdateParameter parameters)
		{
			SystemAdmin admin = DaidanControllersHelper.IdentityUserToSystemAdmin(User.Identity);

			bool result = dbRepository.PONumberBatchUpdate(parameters.TripsIds, parameters.PONumber, admin.Id);
			return Json(result);
		}
		
		[HttpPost]
		[RedirectAuthorize(Roles = "admin, systemAdmin")]
		public ActionResult SellingPriceBatchUpdate(SellingPriceBatchUpdateParameter parameters)
		{
			SystemAdmin admin = DaidanControllersHelper.IdentityUserToSystemAdmin(User.Identity);			

			bool result = dbRepository.SellingPriceBatchUpdate(parameters.TripsIds, parameters.SellingPrice, admin.Id);
			return Json(result);
		}
    }

	public class PONumberBatchUpdateParameter
	{
		public PONumberBatchUpdateParameter()
		{
			this.TripsIds = new List<long>();
		}
		public IList<long> TripsIds { get; set; }
		public string PONumber { get; set; }
	}

	public class SellingPriceBatchUpdateParameter
	{
		public SellingPriceBatchUpdateParameter()
		{
			this.TripsIds = new List<long>();
		}
		public IList<long> TripsIds { get; set; }
		public decimal SellingPrice { get; set; }
	}
}