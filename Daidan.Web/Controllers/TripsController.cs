using Daidan.Domain;
using Daidan.Entities;
using Daidan.Web.Helpers;
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
        // GET: Trips
        public ActionResult Add()
        {
			AddTripViewModel viewModel = new AddTripViewModel();
			viewModel.TripsList = dbRepository.GetAddedTodayTrips();
			viewModel.AddTripLookup = DaidanControllersHelper.GetTripLookups(dbRepository);

			return View(viewModel);
        }

		[HttpPost]
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
		public ActionResult DeletedTrip(long tripId)
		{
			dbRepository.DeleteTrip(tripId);
			return Json("", JsonRequestBehavior.AllowGet);
		}

		public ActionResult GetTrip(long tripId)
		{
			return Json(dbRepository.GetTripById(tripId), JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public ActionResult PONumberBatchUpdate(PONumberBatchUpdateParameter parameters)
		{
			bool result = dbRepository.PONumberBatchUpdate(parameters.TripsIds, parameters.PONumber);
			return Json(result);
		}

		public ActionResult SellingPriceBatchUpdate(SellingPriceBatchUpdateParameter parameters)
		{
			bool result = dbRepository.SellingPriceBatchUpdate(parameters.TripsIds, parameters.SellingPrice);
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