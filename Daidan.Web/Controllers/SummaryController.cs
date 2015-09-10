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
	[RedirectAuthorize(Roles = "systemAdmin")]
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
				item.GrossProfit = monthTripsTruckGroupItem.Sum(x => x.TripGrossProfit);
				item.NetProfit = monthTripsTruckGroupItem.Sum(x => x.TripNetProfit);
				item.AdminFees = monthTripsTruckGroupItem.Sum(x => x.AdminFeesAmount);
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
						item.DriversBasicSalary += driverTripSalaryCost * truckDriversGroupItem.Count();
					}
				}

				viewModel.SummaryItems.Add(item);
			}

			return View(viewModel);
		}

		public ViewResult ByDriver(int month, int year)
		{
			MonthSummaryByDriverViewModel model = new MonthSummaryByDriverViewModel();
			model.Month = new DateTime(year, month, 1);

			IList<Trip> monthTrips = dbRepository.GetMonthTrips(month, year).Where(x => !x.Truck.IsOutsourced).ToList();
			DaidanControllersHelper.UpdateTripsAdminPercentage(monthTrips, month, year, dbRepository);

			IList<TruckExpense> monthDriversTrucksExpenses = dbRepository.GetMonthTruckExpenses(month, year).Where(x => x.Driver != null).ToList();
			IList<DriverSalary> monthDriversSalaries = dbRepository.GetMonthDriverSalaries(month, year);
			IList<DriverCash> monthDriversRecievedCash = dbRepository.GetMonthDriverRecievedCash(month, year);

			model.MonthExpensesSections = monthDriversTrucksExpenses.Select(x => x.Section).Distinct().ToArray();

			IEnumerable<IGrouping<Driver, Trip>> monthTripsDriverGroups = monthTrips.GroupBy(x => x.Driver);

			
			model.SummaryItems = new List<MonthSummaryByDriverViewModel.SummaryItem>();
			foreach (IGrouping<Driver, Trip> monthTripsDriverGroupItem in monthTripsDriverGroups)
			{
				MonthSummaryByDriverViewModel.SummaryItem item = new MonthSummaryByDriverViewModel.SummaryItem();
				item.Driver = monthTripsDriverGroupItem.Key;
				item.NofTrips = monthTripsDriverGroupItem.Count();
				item.GrossProfit = monthTripsDriverGroupItem.Sum(x => x.TripGrossProfit);
				item.AdminFees = monthTripsDriverGroupItem.Sum(x => x.AdminFeesAmount);

				IEnumerable<TruckExpense> driverPaidExpense = monthDriversTrucksExpenses.Where(x => x.Driver.Id == monthTripsDriverGroupItem.Key.Id);
				item.PaidTruckExpenses = new TruckExpense[model.MonthExpensesSections.Length];
				for (int index = 0; index < item.PaidTruckExpenses.Length; index++)
				{
					IEnumerable<TruckExpense> driverSectionExpens = driverPaidExpense.Where(x => x.Section.Id == model.MonthExpensesSections[index].Id);
					if(driverSectionExpens.Count() > 0)
					{
						item.PaidTruckExpenses[index] = new TruckExpense { Section = model.MonthExpensesSections[index], Amount = driverSectionExpens.Sum(x => x.Amount) };
					}
					else
					{
						item.PaidTruckExpenses[index] = new TruckExpense { Section = model.MonthExpensesSections[index], Amount = 0 };
					}
				}
				DriverSalary salary = monthDriversSalaries.FirstOrDefault(x => x.Driver.Id == monthTripsDriverGroupItem.Key.Id);
				if (salary != null)
					item.Salary = salary.Amount;

				item.RecievedCash = monthDriversRecievedCash.Where(x => x.Driver.Id == monthTripsDriverGroupItem.Key.Id).Sum(x => x.Amount);

				model.SummaryItems.Add(item);
			}

			model.SummaryItems = model.SummaryItems.OrderByDescending(x => x.ResultPlusAdmin).ToList();
			return View(model);
		}
    }
}