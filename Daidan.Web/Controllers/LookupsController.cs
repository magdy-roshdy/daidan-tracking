using Daidan.Domain;
using Daidan.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Daidan.Web.Controllers
{
    public class LookupsController : Controller
    {
        private IDataRepository dbRepository;
		public LookupsController(IDataRepository repo)
		{
			dbRepository = repo;
		}

		public ActionResult ListDrivers()
		{
			IList<Driver> drivers = dbRepository.GetAllDrivers();
			return View(drivers);
		}

		public ViewResult EditDriver(int id)
		{
			Driver driver = dbRepository.GetDriverById(id);
			return View(driver);
		}

		public ViewResult CreateDriver()
		{
			return View("EditDriver", new Driver());
		}

		[HttpPost]
		public ActionResult EditDriver(Driver driver)
		{
			if (ModelState.IsValid)
			{
				dbRepository.SaveDriver(driver);
				TempData["message"] = driver.Id > 0 ? "Driver information updated successfully" : "Driver added successfully";
				TempData["message-class"] = "alert-success";

				return RedirectToAction("ListDrivers");
			}
			else
			{
				return View(driver);
			}
		}

		[HttpPost]
		public ActionResult DeleteDriver(int driverId)
		{
			bool result = dbRepository.DeleteDriver(driverId);
			if (result)
			{
				TempData["message"] = "Driver deleted successfully";
				TempData["message-class"] = "alert-success";
			}
			else
			{
				TempData["message"] = "Can't delete this driver";
				TempData["message-class"] = "alert-danger";
			}
			return RedirectToAction("ListDrivers");
		}
    }
}