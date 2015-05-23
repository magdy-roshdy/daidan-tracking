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
    }
}