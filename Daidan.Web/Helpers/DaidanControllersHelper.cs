using Daidan.Domain;
using Daidan.Entities;
using Daidan.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Daidan.Web.Helpers
{
	public class DaidanControllersHelper
	{
		public static AddTripLookup GetTripLookups(IDataRepository dbRepository)
		{
			AddTripLookup lookups = new AddTripLookup();
			lookups.Customers = dbRepository.GetAllCustomers();
			lookups.Drivers = dbRepository.GetAllDrivers();
			lookups.Trucks = dbRepository.GetAllTrucks();
			lookups.Materials = dbRepository.GetAllMaterials();
			lookups.Units = dbRepository.GetAllUnits();
			lookups.Sites = dbRepository.GetAllSites();
			lookups.LastInsertionDate = DateTime.Now;

			return lookups;
		}

		public static List<SelectListItem> CustomersToSelectListItems(IList<Customer> customers, int selectedId = -1, bool addEmptyItem = false)
		{
			List<SelectListItem> selectListItems = new List<SelectListItem>();

			if (addEmptyItem)
			{
				selectListItems.Add(new SelectListItem { Text = "", Value = "0" });
			}

			foreach (Customer customer in customers)
			{
				selectListItems.Add(new SelectListItem { Text = customer.Name, Value = customer.Id.ToString(), Selected = (selectedId == customer.Id) });
			}

			return selectListItems;
		}
	}
}