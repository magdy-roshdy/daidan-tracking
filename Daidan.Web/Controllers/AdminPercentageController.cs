using Daidan.Domain;
using Daidan.Entities;
using Daidan.Web.Infrastructure;
using Daidan.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Daidan.Web.Controllers
{
	[RedirectAuthorize(Roles = "systemAdmin")]
    public class AdminPercentageController : Controller
    {
        private IDataRepository dbRepository;
		public AdminPercentageController(IDataRepository repo)
		{
			dbRepository = repo;
		}

		public ViewResult List()
		{
			IList<MonthAminPercentage> monthsList = dbRepository.GetAllMonthAdminPercentages();
			return View(monthsList);
		}

		public ViewResult Manage(int id)
		{
			MonthAminPercentage model = dbRepository.GetMonthAminPercentageById(id);
			return View(model);
		}

		public ActionResult GetMonthMaterials(int monthId)
		{
			AdminPercentageEditViewModel model = new AdminPercentageEditViewModel();
			model.MaterialPercentages = dbRepository.GetMaterialsAdminPercentageByMonth(monthId);
			model.MaterialsList = dbRepository.GetAllMaterials();
			model.CustomersList = dbRepository.GetAllCustomers();
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		public ActionResult SaveMonthPercentages(IList<MaterialAdminPercentage> monthMaterialPercentages, int monthId,
			IList<long> materialsToDelete, IList<long> customersToDelete)
		{
			if (materialsToDelete == null)
				materialsToDelete = new List<long>();

			if (customersToDelete == null)
				customersToDelete = new List<long>();

			dbRepository.SaveMonthAdminPercentage(new MonthAminPercentage { Id = monthId }, monthMaterialPercentages, materialsToDelete, customersToDelete);

			TempData["message"] = "Month percentage information successfully";
			TempData["message-class"] = "alert-success";
			return Json(new { result = true, url = Url.Action("List", "AdminPercentage") });
		}

		public ViewResult EditMonth(int id)
		{
			MonthAdminPercentageEditViewModel model = new MonthAdminPercentageEditViewModel();
			model.Month = dbRepository.GetMonthAminPercentageById(id);
			model.Months = Helpers.DaidanControllersHelper.YearMonthsToSelectListItems(model.Month.Month);
			return View(model);
		}

		[HttpPost]
		public ActionResult EditMonth(MonthAdminPercentageEditViewModel model)
		{
			if (ModelState.IsValid)
			{
				MonthAminPercentage month = new MonthAminPercentage();
				month.Id = model.Month.Id;
				month.Year = model.Month.Year;
				month.Month = model.Month.Month;
				month.Amount = model.Month.Amount;

				try
				{
					dbRepository.SaveMonthAdminPercentage(month, model.MonthToCopyFrom);

					TempData["message"] = model.Month.Id > 0 ? "Month updated successfully" : "Month added successfully";
					TempData["message-class"] = "alert-success";

					return RedirectToAction("List", "AdminPercentage", new { id = model.Month.Id });
				}
				catch(Exception)
				{
					TempData["message"] = "Can't save changes, may be there an existing month with the same year and month you entered!";
					TempData["message-class"] = "alert-danger";

					model.Months = Helpers.DaidanControllersHelper.YearMonthsToSelectListItems(model.Month.Month);
					model.PreviouseMonths = this.getPreviousMonthlist();
					return View(model);
				}
			}
			else
			{
				model.Months = Helpers.DaidanControllersHelper.YearMonthsToSelectListItems(model.Month.Month);
				model.PreviouseMonths = this.getPreviousMonthlist();
				return View(model);
			}
		}

		public ViewResult CreateMonth()
		{
			MonthAdminPercentageEditViewModel model = new MonthAdminPercentageEditViewModel();
			model.Month = new MonthAminPercentage { Year = DateTime.Now.Year, Amount = decimal.Parse(ConfigurationManager.AppSettings["globalPercentage"]), Month = DateTime.Now.Month };
			model.Months = Helpers.DaidanControllersHelper.YearMonthsToSelectListItems(model.Month.Month);
			model.PreviouseMonths = this.getPreviousMonthlist();

			return View("EditMonth", model);
		}

		public ActionResult DeleteMonth(int monthId)
		{
			bool result = dbRepository.DeleteMonthAdminPercentage(monthId);
			if (result)
			{
				TempData["message"] = "Month deleted successfully";
				TempData["message-class"] = "alert-success";
			}
			else
			{
				TempData["message"] = "Can't delete this month!";
				TempData["message-class"] = "alert-danger";
			}

			return RedirectToAction("List");
		}

		private List<SelectListItem> getPreviousMonthlist()
		{
			IList<MonthAminPercentage> monthsList = dbRepository.GetAllMonthAdminPercentages();
			List<SelectListItem> list = new List<SelectListItem>();
			list.Add(new SelectListItem { Text = "[SELECT]", Value = "0" });
			foreach (MonthAminPercentage month in monthsList)
			{
				list.Add(new SelectListItem { Text = month.MonthCaption, Value = month.Id.ToString() });
			}

			return list;
		}
    }
}