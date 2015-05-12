using Daidan.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Daidan.Web.Models
{
	public class AddTripLookup
	{
		public DateTime LastInsertionDate { get; set; }
		public IList<Driver> Drivers { get; set; }
		public IList<Truck> Trucks { get; set; }
		public IList<Unit> Units { get; set; }
		public IList<Material> Materials { get; set; }
		public IList<Customer> Customers { get; set; }
		public IList<Site> Sites { get; set; }
	}
}