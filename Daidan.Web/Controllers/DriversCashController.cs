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
    public class DriversCashController : Controller
    {
		private IDataRepository dbRepository;
		public DriversCashController(IDataRepository repo)
		{
			dbRepository = repo;
		}

        public ActionResult Index()
        {
			DriversCashIndexViewModel model = new DriversCashIndexViewModel();
			model.Drivers = Helpers.DaidanControllersHelper.DriversToSelectListItems(dbRepository.GetAllDrivers());
			model.From = new DateTime(DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month, 1);
			model.To = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddDays(-1);
			return View(model);
        }

		[HttpPost]
		public ActionResult Index(DriversCashIndexViewModel model)
		{
			if(ModelState.IsValid)
			{
				DriverCashListViewModel listModel = new DriverCashListViewModel();
				listModel.DriverId = model.DriverId;
				listModel.From = model.From.Value;
				listModel.To = model.To.Value;
				listModel.DriverName = dbRepository.GetDriverById(listModel.DriverId).Name;

				listModel.DriverCashList = dbRepository.GetDriverCashList(listModel.DriverId, listModel.From, listModel.To);
				return View("List", listModel);
			}
			else
			{
				model.Drivers = Helpers.DaidanControllersHelper.DriversToSelectListItems(dbRepository.GetAllDrivers(), model.DriverId);
				return View(model);
			}
		}

		public ActionResult Create(int? driverId)
		{
			DriverCashEditViewModel model = new DriverCashEditViewModel();
			if (driverId.HasValue)
				model.DriverId = driverId.Value;

			model.Drivers = Helpers.DaidanControllersHelper.DriversToSelectListItems(dbRepository.GetAllDrivers(), driverId.HasValue ? driverId.Value : -1);

			return View("Edit", model);
		}

		public ActionResult Edit(long id)
		{
			DriverCash db_cash = dbRepository.GetDriverCashById(id);
			DriverCashEditViewModel model = new DriverCashEditViewModel();
			model.DriverCashId = db_cash.Id;
			model.DriverId = db_cash.Driver.Id;
			model.Date = db_cash.Date;
			model.VoucherNumber = db_cash.VoucherNumber;
			model.Amount = db_cash.Amount;

			model.Drivers = Helpers.DaidanControllersHelper.DriversToSelectListItems(dbRepository.GetAllDrivers(), model.DriverId);

			return View(model);
		}

		[HttpPost]
		public ActionResult Edit(DriverCashEditViewModel model)
		{
			if(this.ModelState.IsValid)
			{
				DriverCash cash = new DriverCash();
				cash.Id = model.DriverCashId;
				cash.Driver = new Driver { Id = model.DriverId };
				cash.Date = model.Date.Value;
				cash.Amount = model.Amount.Value;
				cash.VoucherNumber = model.VoucherNumber;

				dbRepository.SaveDriverCash(cash);

				TempData["message"] = model.DriverCashId > 0 ? "Driver cash information updated successfully" : "Driver cash added successfully";
				TempData["message-class"] = "alert-success";

				DriverCashListViewModel listModel = new DriverCashListViewModel();
				listModel.DriverId = model.DriverId;
				listModel.From = new DateTime(cash.Date.Year, cash.Date.Month, 1);
				listModel.To = listModel.From.AddMonths(1).AddDays(-1);
				listModel.DriverName = dbRepository.GetDriverById(listModel.DriverId).Name;
				listModel.DriverCashList = dbRepository.GetDriverCashList(listModel.DriverId, listModel.From, listModel.To).OrderBy(x => x.Date).ToList();

				return View("List", listModel);
			}
			else
			{
				model.Drivers = Helpers.DaidanControllersHelper.DriversToSelectListItems(dbRepository.GetAllDrivers(), model.DriverId);
				return View(model);
			}
		}

		public ActionResult Delete(long cashId, int driverId, DateTime dateFrom, DateTime dateTo)
		{
			bool result = dbRepository.DeleteDriverCash(cashId);
			if (result)
			{
				TempData["message"] = "Driver Cash deleted successfully";
				TempData["message-class"] = "alert-success";
			}
			else
			{
				TempData["message"] = "Can't delete this driver cash";
				TempData["message-class"] = "alert-danger";
			}

			DriverCashListViewModel listModel = new DriverCashListViewModel();
			listModel.DriverId = driverId;
			listModel.From = dateFrom;
			listModel.To = dateTo;
			listModel.DriverName = dbRepository.GetDriverById(listModel.DriverId).Name;
			listModel.DriverCashList = dbRepository.GetDriverCashList(listModel.DriverId, listModel.From, listModel.To).OrderBy(x => x.Date).ToList();

			return View("List", listModel);
		}
    }
}