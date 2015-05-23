using Daidan.Domain;
using Daidan.Entities;
using Daidan.Web.Models;
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

		//materials
		public ActionResult MaterialsList()
		{
			return View(dbRepository.GetAllMaterials());
		}

		public ActionResult EditMaterial(int id)
		{
			Material material = dbRepository.GetMaterialById(id);
			return View(material);
		}

		[HttpPost]
		public ActionResult EditMaterial(Material material)
		{
			if (ModelState.IsValid)
			{
				dbRepository.SaveMaterial(material);
				TempData["message"] = material.Id > 0 ? "Material information updated successfully" : "Material added successfully";
				TempData["message-class"] = "alert-success";

				return RedirectToAction("MaterialsList");
			}
			else
			{
				return View(material);
			}
		}

		public ViewResult CreateMaterial()
		{
			return View("EditMaterial", new Material());
		}

		[HttpPost]
		public ActionResult DeleteMaterial(int materialId)
		{
			bool result = dbRepository.DeleteMaterial(materialId);
			if (result)
			{
				TempData["message"] = "Material deleted successfully";
				TempData["message-class"] = "alert-success";
			}
			else
			{
				TempData["message"] = "Can't delete this material";
				TempData["message-class"] = "alert-danger";
			}
			return RedirectToAction("MaterialsList");
		}

		//customers
		public ActionResult CustomersList()
		{
			return View(dbRepository.GetAllCustomers());
		}

		public ViewResult EditCustomer(int id)
		{
			Customer customer = dbRepository.GetCustomerById(id);
			return View(customer);
		}

		[HttpPost]
		public ActionResult EditCustomer(Customer customer)
		{
			if (ModelState.IsValid)
			{
				dbRepository.SaveCustomer(customer);
				TempData["message"] = customer.Id > 0 ? "Customer information updated successfully" : "Customer added successfully";
				TempData["message-class"] = "alert-success";

				return RedirectToAction("CustomersList");
			}
			else
			{
				return View(customer);
			}
		}

		public ViewResult CreateCustomer()
		{
			return View("EditCustomer", new Customer());
		}

		[HttpPost]
		public ActionResult DeleteCustomer(int customerId)
		{
			bool result = dbRepository.DeleteCustomer(customerId);
			if (result)
			{
				TempData["message"] = "Customer deleted successfully";
				TempData["message-class"] = "alert-success";
			}
			else
			{
				TempData["message"] = "Can't delete this customer";
				TempData["message-class"] = "alert-danger";
			}
			return RedirectToAction("CustomersList");
		}

		//sites
		public ActionResult SitesList(int id)
		{
			SitesListViewModel model = new SitesListViewModel();
			model.Sites = dbRepository.GetSitesByCustomerId(id);
			model.Customer = dbRepository.GetCustomerById(id);
			
			return View(model);
		}

		public ViewResult EditSite(int id)
		{
			EditSiteViewModel model = new EditSiteViewModel();
			model.Customers = dbRepository.GetAllCustomers();
			Site site = dbRepository.GetSiteById(id);

			model.SiteId = site.Id;
			model.SiteName = site.Name;
			model.SiteIsActive = site.IsActive;
			model.SiteIsOwnSite = site.IsOwnSite;
			model.CustomerId = site.Customer.Id;

			return View(model);
		}

		[HttpPost]
		public ActionResult EditSite(EditSiteViewModel model)
		{
			if (ModelState.IsValid)
			{
				Site site = new Site();
				site.Id = model.SiteId;
				site.Name = model.SiteName;
				site.IsActive = model.SiteIsActive;
				site.IsOwnSite = model.SiteIsOwnSite;
				site.Customer = new Customer { Id = model.CustomerId };

				dbRepository.SaveSite(site);
				TempData["message"] = site.Id > 0 ? "Site information updated successfully" : "Site added successfully";
				TempData["message-class"] = "alert-success";

				return RedirectToAction("SitesList", new { id = model.CustomerId });
			}
			else
			{
				model.Customers = dbRepository.GetAllCustomers();
				return View(model);
			}
		}

		public ViewResult CreateSite(int id)
		{
			EditSiteViewModel model = new EditSiteViewModel();
			model.Customers = dbRepository.GetAllCustomers();
			model.CustomerId = id;

			return View("EditSite", model);
		}

		[HttpPost]
		public ActionResult DeleteSite(int siteId)
		{
			int customerId = dbRepository.GetSiteById(siteId).Customer.Id;
			bool result = dbRepository.DeleteSite(siteId);
			if (result)
			{
				TempData["message"] = "Site deleted successfully";
				TempData["message-class"] = "alert-success";
			}
			else
			{
				TempData["message"] = "Can't delete this site";
				TempData["message-class"] = "alert-danger";
			}
			return RedirectToAction("SitesList", new { id = customerId });
		}

		//trucks
		public ActionResult TrucksList()
		{
			return View(dbRepository.GetAllTrucks());
		}

		public ActionResult EditTruck(int id)
		{
			EditTruckViewModel model = new EditTruckViewModel();
			Truck db_truck = dbRepository.GetTruckById(id);

			model.TruckId = db_truck.Id;
			model.TruckNumber = db_truck.Number;
			model.TruckIsActive = db_truck.IsActive;
			model.TruckIsOutsourced = db_truck.IsOutsourced;
			model.TruckDriverId = db_truck.Driver != null ? db_truck.Driver.Id : 0;

			model.Drivers = getEditTruckDriversList(db_truck);
			return View(model);
		}

		[HttpPost]
		public ActionResult EditTruck(EditTruckViewModel model)
		{
			Truck truck = new Truck();
			truck.Id = model.TruckId;
			truck.Number = model.TruckNumber;
			truck.IsActive = model.TruckIsActive;
			truck.IsOutsourced = model.TruckIsOutsourced;
			if (model.TruckDriverId > 0)
				truck.Driver = new Driver { Id = model.TruckDriverId };

			if (ModelState.IsValid)
			{
				dbRepository.SaveTruck(truck);
				TempData["message"] = truck.Id > 0 ? "Truck information updated successfully" : "Truck added successfully";
				TempData["message-class"] = "alert-success";

				return RedirectToAction("TrucksList");
			}
			else
			{
				if (truck.Driver != null)
					truck.Driver = dbRepository.GetDriverById(truck.Driver.Id);
				model.TruckIsOutsourcedX = model.TruckIsOutsourced;
				model.Drivers = getEditTruckDriversList(truck);
				return View(model);
			}
		}

		public ActionResult CreateTruck(bool isOutsourced)
		{
			EditTruckViewModel model = new EditTruckViewModel();
			model.TruckIsOutsourced = isOutsourced;
			model.Drivers = getEditTruckDriversList(new Truck { IsOutsourced = isOutsourced });

			return View("EditTruck", model);
		}

		private List<SelectListItem> getEditTruckDriversList(Truck truck)
		{			
			IList<Driver> drivers = new List<Driver>();
			if (truck.IsOutsourced)
				drivers = dbRepository.GetAllDrivers().Where(x => x.IsOutsourced).ToList<Driver>();
			else
			{
				drivers = dbRepository.GetTruckFreeDrivers(null);
				if (truck.Driver != null)
					drivers.Add(truck.Driver);
			}

			return Helpers.DaidanControllersHelper.DriversToSelectListItems(drivers, truck.Driver != null ? truck.Driver.Id : 0, true);
		}

		[HttpPost]
		public ActionResult DeleteTruck(int truckId)
		{
			bool result = dbRepository.DeleteTruck(truckId);
			if (result)
			{
				TempData["message"] = "Truck deleted successfully";
				TempData["message-class"] = "alert-success";
			}
			else
			{
				TempData["message"] = "Can't delete this truck!";
				TempData["message-class"] = "alert-danger";
			}
			return RedirectToAction("TrucksList");
		}
    }
}