using Daidan.Domain;
using Daidan.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
	}
}