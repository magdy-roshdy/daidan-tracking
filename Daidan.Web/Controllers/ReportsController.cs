using Daidan.Domain;
using Daidan.Entities;
using Daidan.Web.Helpers;
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

		public ActionResult TruckSheet()
		{
			TrucksExpensesIndexViewModel model = new TrucksExpensesIndexViewModel();
			model.Trucks = Helpers.DaidanControllersHelper.TrucksToSelectListItems(dbRepository.GetAllTrucks());
			model.Months = Helpers.DaidanControllersHelper.YearMonthsToSelectListItems();
			model.Year = DateTime.Now.Year;

			return View(model);
		}

		[HttpPost]
		public ActionResult TruckSheetSearch(TruckSheetSearchParameter searchParameter)
		{
			TruckSheetResultViewModel result = new TruckSheetResultViewModel();
			
			MasterReportSearchParameters tripsSearchParameters = new MasterReportSearchParameters();
			tripsSearchParameters.TruckId = searchParameter.TruckId;
			tripsSearchParameters.From = new DateTime(searchParameter.Year, searchParameter.Month, 1);
			tripsSearchParameters.To = tripsSearchParameters.From.Value.AddMonths(1).AddDays(-1);
			result.Trips = dbRepository.MasterReportSearch(tripsSearchParameters).OrderBy(x => x.Date).ToList();

			IList<TruckExpense> expenses = dbRepository.GetTruckSheetExpenses(searchParameter.TruckId, searchParameter.Month, searchParameter.Year);
			IList<TruckExpense> grupedExpenses = new List<TruckExpense>();

			foreach (TruckExpense expense in expenses)
			{
				TruckExpense e = grupedExpenses.FirstOrDefault(x => x.Section.Id == expense.Section.Id);
				if(e != null)
				{
					e.Amount += expense.Amount;
				}
				else
				{
					grupedExpenses.Add(expense);
				}
			}

			result.Expenses = grupedExpenses;
			return Json(result);
		}
    }
}