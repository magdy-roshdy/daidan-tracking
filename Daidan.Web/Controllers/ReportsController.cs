using Daidan.Domain;
using Daidan.Entities;
using Daidan.Web.Helpers;
using Daidan.Web.Infrastructure;
using Daidan.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
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

		public ActionResult CustomerReport()
		{
			return View(DaidanControllersHelper.GetTripLookups(dbRepository));
		}

		public ActionResult TruckSheet()
		{
			TrucksExpensesIndexViewModel model = new TrucksExpensesIndexViewModel();
			model.Trucks = Helpers.DaidanControllersHelper.TrucksToSelectListItems(dbRepository.GetAllTrucks());
			model.Months = Helpers.DaidanControllersHelper.YearMonthsToSelectListItems();
			model.Month = DateTime.Now.AddMonths(-1).Month;
			model.Year = DateTime.Now.AddMonths(-1).Year;

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

			//update admin perecntage
			Helpers.DaidanControllersHelper.UpdateTripsAdminPercentage(result.Trips, searchParameter.Month, searchParameter.Year, dbRepository);

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

		public ActionResult DriverSheet()
		{
			DriverSheetIndexViewModel model = new DriverSheetIndexViewModel();
			model.Drivers = Helpers.DaidanControllersHelper.DriversToSelectListItems(dbRepository.GetAllDrivers());
			model.Months = Helpers.DaidanControllersHelper.YearMonthsToSelectListItems();
			model.Month = DateTime.Now.AddMonths(-1).Month;
			model.Year = DateTime.Now.AddMonths(-1).Year;

			return View(model);
		}

		[HttpPost]
		public ActionResult DriverSheetSearch(DriverSheetSearchParameter searchParameter)
		{
			DriverSheetResultViewModel result = new DriverSheetResultViewModel();

			MasterReportSearchParameters tripsSearchParameters = new MasterReportSearchParameters();
			tripsSearchParameters.DriverId = searchParameter.DriverId;
			tripsSearchParameters.From = new DateTime(searchParameter.Year, searchParameter.Month, 1);
			tripsSearchParameters.To = tripsSearchParameters.From.Value.AddMonths(1).AddDays(-1);
			result.Trips = dbRepository.MasterReportSearch(tripsSearchParameters).OrderBy(x => x.Date).ToList();

			//update admin perecntage
			Helpers.DaidanControllersHelper.UpdateTripsAdminPercentage(result.Trips, searchParameter.Month, searchParameter.Year, dbRepository);

			decimal profitPercent = 0.1M;
			decimal.TryParse(ConfigurationManager.AppSettings["driverProfitPercentage"], out profitPercent);
			foreach (Trip trip in result.Trips)
			{
				if (trip.TripNetProfit > 0)
					result.DriverTripsProfit += trip.TripNetProfit * profitPercent;
			}

			DriverSalary monthSalary = dbRepository.GetDriverMonthSalary(searchParameter.DriverId, searchParameter.Month, searchParameter.Year);
			if (monthSalary != null)
				result.MonthSalary = monthSalary.Amount;

			DateTime previousMonth = new DateTime(searchParameter.Year, searchParameter.Month, 1).AddMonths(-1);
			result.PreviousMonthBalanace = getDriverMonthBalance(searchParameter.DriverId, previousMonth.Month, previousMonth.Year);

			IList<TruckExpense> expenses = dbRepository.GetTruckExpensesByDriver(searchParameter.DriverId, searchParameter.Month, searchParameter.Year);
			result.MonthPaidExpensesSum = expenses.Sum(x => x.Amount);

			IEnumerable<IGrouping<ExpensesSection, TruckExpense>> groups = expenses.GroupBy(x => x.Section);
			foreach (IGrouping<ExpensesSection, TruckExpense> group in groups)
			{
				result.MonthPaidExpenses.Add(new TruckExpense { Section = group.Key, Amount = group.Sum(x => x.Amount) });
			}

			result.ExtraCostSum = result.Trips.Sum(x => x.ExtraCost);

			result.MonthDriverCash = dbRepository.GetDriverCashList(searchParameter.DriverId, tripsSearchParameters.From,  tripsSearchParameters.To).OrderBy(x => x.Date).ToList();
			result.MonthDriverCashSum = result.MonthDriverCash.Sum(x => x.Amount);

			//this to prevent update of May 2015 and prior, since the system started production at June 2015
			if(DateTime.Compare(new DateTime(searchParameter.Year, searchParameter.Month, 1), new DateTime(2015, 5, 1)) > 0)
				dbRepository.UpdateDriverMonthBalanace(searchParameter.DriverId, searchParameter.Month, searchParameter.Year, result.MonthBalance);

			return Json(result);
		}

		private decimal getDriverMonthBalance(int driverId, int month, int year)
		{
			DriverMonthBalance balance = dbRepository.GetDriverMonthBalance(driverId, month, year);
			if (balance != null)
				return balance.Amount;
			else
				return 0;
		}
    }
}