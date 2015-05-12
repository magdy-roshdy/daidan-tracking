using Daidan.Domain;
using Daidan.Entities;
using Daidan.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
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



			viewModel.AddTripLookup = GetTripLookups();

			return View(viewModel);
        }

		[HttpPost]
		public ActionResult SaveTrip(Trip trip)
		{
			if(trip != null)
			{
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

		public ActionResult MasterReport()
		{
			return View(GetTripLookups());
		}

		[HttpPost]
		public ActionResult MasterReportSearch(MasterReportSearchParameters parameter)
		{
			return Json(dbRepository.MasterReportSearch(parameter), JsonRequestBehavior.AllowGet);
		}

		private AddTripLookup GetTripLookups()
		{
			AddTripLookup lookups = new AddTripLookup();
			lookups.Customers = dbRepository.GetAllCustomers();
			lookups.Drivers = dbRepository.GetAllDrivers();
			lookups.Trucks = dbRepository.GetAllTrucks();
			lookups.Materials = dbRepository.GetAllMaterials();
			lookups.Units = dbRepository.GetAllUnits();
			lookups.Sites = dbRepository.GetAllSites();
			lookups.LastInsertionDate = DateTime.Now;

			return lookups;
		}
    }
}