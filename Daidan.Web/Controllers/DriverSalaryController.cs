using Daidan.Domain;
using Daidan.Entities;
using Daidan.Web.Infrastructure;
using Daidan.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Daidan.Web.Controllers
{
	[RedirectAuthorize(Roles = "admin, systemAdmin")]
    public class DriverSalaryController : Controller
    {
       private IDataRepository dbRepository;
	   public DriverSalaryController(IDataRepository repo)
		{
			dbRepository = repo;
		}

		public ViewResult List(int id)
		{
			DriverSalariesListViewModel model = new DriverSalariesListViewModel();
			model.Salaries = dbRepository.GetDriverSalaries(id);
			model.Driver = dbRepository.GetDriverById(id);
			
			return View(model);
		}

		public ViewResult Edit(long id)
		{
			DriverSalaryEditViewModel model = new DriverSalaryEditViewModel();
			DriverSalary salary = dbRepository.GetDriverSalaryById(id);

			model.DriverSalaryId = salary.Id;
			model.Driver = salary.Driver;
			model.DriverId = salary.Driver.Id;
			model.Amount = salary.Amount;
			model.Month = salary.Month;
			model.Year = salary.Year;
			model.Months = Helpers.DaidanControllersHelper.YearMonthsToSelectListItems();

			return View(model);
		}

		[HttpPost]
		public ActionResult Edit(DriverSalaryEditViewModel model)
		{
			if (ModelState.IsValid)
			{
				DriverSalary salary = new DriverSalary();
				salary.Id = model.DriverSalaryId;
				salary.Driver = new Driver { Id=model.DriverId };
				salary.Month = model.Month;
				salary.Year = model.Year;
				salary.Amount = model.Amount;

				try
				{ 
					dbRepository.SaveDriverSalary(salary);
					TempData["message"] = model.DriverSalaryId > 0 ? "Driver salary information updated successfully" : "Driver salary added successfully";
					TempData["message-class"] = "alert-success";
				}
				catch(Exception)
				{
					TempData["message"] = "Can't save the salary, may be there is a salary with the same month for this driver!";
					TempData["message-class"] = "alert-danger";
				}


				return RedirectToAction("List", "DriverSalary", new { id = model.DriverId });
			}
			else
			{
				model.Months = Helpers.DaidanControllersHelper.YearMonthsToSelectListItems();
				model.Driver = dbRepository.GetDriverById(model.DriverId);
				return View(model);
			}
		}

		public ViewResult Create(int id)
		{
			DriverSalaryEditViewModel model = new DriverSalaryEditViewModel();
			model.Driver = dbRepository.GetDriverById(id);

			model.Month = DateTime.Now.Month;
			model.Year = DateTime.Now.Year;
			model.Months = Helpers.DaidanControllersHelper.YearMonthsToSelectListItems();
			model.DriverId = model.Driver.Id;

			return View("Edit", model);
		}

		public ActionResult Delete(long salaryId, int driverId)
		{
			bool result = dbRepository.DeleteDriverSalary(salaryId);
			if (result)
			{
				TempData["message"] = "Driver salary deleted successfully";
				TempData["message-class"] = "alert-success";
			}
			else
			{
				TempData["message"] = "Can't delete driver salary!";
				TempData["message-class"] = "alert-danger";
			}

			return RedirectToAction("List", "DriverSalary", new { id = driverId });
		}
    }
}