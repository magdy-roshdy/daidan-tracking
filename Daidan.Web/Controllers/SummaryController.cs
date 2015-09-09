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
    public class SummaryController : Controller
    {
        private IDataRepository dbRepository;
		public SummaryController(IDataRepository repo)
		{
			dbRepository = repo;
		}

		public ViewResult Index()
		{
			IList<DateTime> monthsList = dbRepository.GetDistinctMonths();
			return View(monthsList);
		}

		public ViewResult ByTruck(int month, int year)
		{
			IList<Trip> monthTrips = dbRepository.GetMonthTrips(month, year).Where(x => !x.Truck.IsOutsourced).ToList();
			DaidanControllersHelper.UpdateTripsAdminPercentage(monthTrips, month, year, dbRepository);

			IList<TruckExpense> monthTrucksExpenses = dbRepository.GetMonthTruckExpenses(month, year);
			IList<DriverSalary> driversSalaries = dbRepository.GetMonthDriverSalaries(month, year);

			MonthSummaryByTruckViewModel viewModel = new MonthSummaryByTruckViewModel();
			viewModel.Month = new DateTime(year, month, 1);
			viewModel.SummaryItems = new List<MonthSummaryByTruckViewModel.SummaryItem>();

			IEnumerable<IGrouping<Truck, Trip>> truckGroups = monthTrips.GroupBy(x => x.Truck);
			IEnumerable<IGrouping<Driver, Trip>> monthTripsDriverGroups = monthTrips.GroupBy(x => x.Driver);

			foreach(IGrouping<Truck, Trip> monthTripsTruckGroupItem in truckGroups)
			{
				MonthSummaryByTruckViewModel.SummaryItem item = new MonthSummaryByTruckViewModel.SummaryItem();
				item.Truck = monthTripsTruckGroupItem.Key;
				item.NofTrips = monthTripsTruckGroupItem.Count();
				item.Profit = monthTripsTruckGroupItem.Sum(x => x.TripNetProfit);
				item.Expenses = monthTrucksExpenses.Where(x => x.Truck.Id == item.Truck.Id).Sum(y => y.Amount);

				//calculating salary portions
				IEnumerable<IGrouping<Driver, Trip>> truckTripsDriverGroups = monthTripsTruckGroupItem.GroupBy(x => x.Driver);
				foreach(IGrouping<Driver, Trip> truckDriversGroupItem in truckTripsDriverGroups)
				{
					DriverSalary driverSalary = driversSalaries.FirstOrDefault(x => x.Driver.Id == truckDriversGroupItem.Key.Id);
					if (driverSalary != null && driverSalary.Amount > 0)
					{
						int nofDriverMonthTrips = monthTripsDriverGroups.FirstOrDefault(x => x.Key.Id == truckDriversGroupItem.Key.Id).Count();
						decimal driverTripSalaryCost = driverSalary.Amount / nofDriverMonthTrips;
						item.Expenses += driverTripSalaryCost * truckDriversGroupItem.Count();
					}
				}

				viewModel.SummaryItems.Add(item);
			}

			return View(viewModel);
		}
    }
}