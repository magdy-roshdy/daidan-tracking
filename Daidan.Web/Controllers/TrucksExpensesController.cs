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
    public class TrucksExpensesController : Controller
    {
		private IDataRepository dbRepository;
        public TrucksExpensesController(IDataRepository repo)
		{
			dbRepository = repo;
		}

        public ActionResult Index()
        {
			TrucksExpensesIndexViewModel model = new TrucksExpensesIndexViewModel();
			model.Trucks = Helpers.DaidanControllersHelper.TrucksToSelectListItems(dbRepository.GetAllTrucks());
			model.Months = Helpers.DaidanControllersHelper.YearMonthsToSelectListItems();
			model.Year = DateTime.Now.Year;

			return View(model);
        }

		[HttpPost]
		public ActionResult Index(TrucksExpensesIndexViewModel model)
		{
			if (ModelState.IsValid)
			{
				TrucksExpensesListViewModel listModel = new TrucksExpensesListViewModel();
				listModel.Month = new DateTime(1, model.Month, 1).ToString("MMMM");
				listModel.Year = model.Year;
				listModel.TruckNumber = dbRepository.GetTruckById(model.TruckId).Number;
				listModel.TruckId = model.TruckId;
				listModel.TruckExpenseList = dbRepository.GetTruckExpenses(model.TruckId, model.Month, model.Year);

				return View("List", listModel);
			}
			else
			{
				model.Trucks = Helpers.DaidanControllersHelper.TrucksToSelectListItems(dbRepository.GetAllTrucks(), model.TruckId);
				model.Months = Helpers.DaidanControllersHelper.YearMonthsToSelectListItems(model.Month);

				return View(model);
			}
		}

		public ActionResult Edit(long id)
		{
			TruckExpense db_expense = dbRepository.GetTruckExpenseById(id);

			TrucksExpensesEditViewModel model = new TrucksExpensesEditViewModel();
			model.ExpenseId = db_expense.Id;
			model.Amount = db_expense.Amount;
			model.SectionId = db_expense.Section.Id;
			model.TruckId = db_expense.Truck.Id;
			model.Month = db_expense.Month;
			model.Year = db_expense.Year;
			model.DriverId = db_expense.Driver != null ? (int?)db_expense.Driver.Id : null;

			model.Months = Helpers.DaidanControllersHelper.YearMonthsToSelectListItems(model.Month);
			model.Trucks = Helpers.DaidanControllersHelper.TrucksToSelectListItems(dbRepository.GetAllTrucks(), model.TruckId);
			model.Sections = Helpers.DaidanControllersHelper.TrucksExpenseSectionsToSelectListItems(dbRepository.GetAllTrucksExpensesSections(), model.SectionId);
			model.Drivers = Helpers.DaidanControllersHelper.DriversToSelectListItems(dbRepository.GetAllDrivers(), model.DriverId.HasValue ? model.DriverId.Value : -1, true);

			return View(model);
		}

		[HttpPost]
		public ActionResult Edit(TrucksExpensesEditViewModel model)
		{
			if (ModelState.IsValid)
			{
				TruckExpense expense = new TruckExpense();
				expense.Id = model.ExpenseId;
				expense.Month = model.Month;
				expense.Year = model.Year;
				expense.Section = new ExpensesSection { Id = model.SectionId };
				expense.Driver = model.DriverId.HasValue ?  new Driver { Id = model.DriverId.Value } : null;
				expense.Truck = new Truck { Id = model.TruckId };
				expense.Amount = model.Amount.Value;

				dbRepository.SaveTruckExpense(expense);

				TempData["message"] = model.ExpenseId > 0 ? "Truck expense information updated successfully" : "Truck expense added successfully";
				TempData["message-class"] = "alert-success";

				TrucksExpensesListViewModel listModel = new TrucksExpensesListViewModel();
				listModel.Month = new DateTime(1, model.Month, 1).ToString("MMMM");
				listModel.Year = model.Year;
				listModel.TruckNumber = dbRepository.GetTruckById(model.TruckId).Number;
				listModel.TruckId = model.TruckId;
				listModel.TruckExpenseList = dbRepository.GetTruckExpenses(model.TruckId, model.Month, model.Year);

				return View("List", listModel);
			}
			else
			{
				model.Months = Helpers.DaidanControllersHelper.YearMonthsToSelectListItems(model.Month);
				model.Trucks = Helpers.DaidanControllersHelper.TrucksToSelectListItems(dbRepository.GetAllTrucks(), model.TruckId);
				model.Sections = Helpers.DaidanControllersHelper.TrucksExpenseSectionsToSelectListItems(dbRepository.GetAllTrucksExpensesSections(), model.SectionId);
				model.Drivers = Helpers.DaidanControllersHelper.DriversToSelectListItems(dbRepository.GetAllDrivers(), model.DriverId.HasValue ? model.DriverId.Value : -1, true);
				return View(model);
			}
		}

		public ActionResult Create(int? truckId, int? month, int? year)
		{
			TrucksExpensesEditViewModel model = new TrucksExpensesEditViewModel();
			if(truckId.HasValue && month.HasValue && year.HasValue)
			{
				model.TruckId = truckId.Value;
				model.Year = year.Value;
				model.Month = month.Value;
			}

			model.Months = Helpers.DaidanControllersHelper.YearMonthsToSelectListItems(month.HasValue ? month.Value : -1);
			model.Trucks = Helpers.DaidanControllersHelper.TrucksToSelectListItems(dbRepository.GetAllTrucks(), truckId.HasValue ? truckId.Value : -1);
			model.Sections = Helpers.DaidanControllersHelper.TrucksExpenseSectionsToSelectListItems(dbRepository.GetAllTrucksExpensesSections());
			model.Drivers = Helpers.DaidanControllersHelper.DriversToSelectListItems(dbRepository.GetAllDrivers(), -1, true);
			return View("Edit", model);
		}

		public ActionResult Delete(long expenseId, int truckId, string truckNumber, int month, int year)
		{
			bool result = dbRepository.DeleteTruckExpense(expenseId);
			if (result)
			{
				TempData["message"] = "Truck expense deleted successfully";
				TempData["message-class"] = "alert-success";
			}
			else
			{
				TempData["message"] = "Can't delete this truck expense";
				TempData["message-class"] = "alert-danger";
			}

			TrucksExpensesListViewModel listModel = new TrucksExpensesListViewModel();
			listModel.Month = new DateTime(1, month, 1).ToString("MMMM");
			listModel.Year = year;
			listModel.TruckNumber = truckNumber;
			listModel.TruckId = truckId;
			listModel.TruckExpenseList = dbRepository.GetTruckExpenses(truckId, month, year);

			return View("List", listModel);
		}
    }
}