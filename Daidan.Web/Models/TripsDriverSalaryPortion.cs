using Daidan.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Daidan.Web.Models
{
	public class TripsDriverSalaryPortion
	{
		public Driver Driver { get; set; }
		public int NumberOfTrips { get; set; }
		public decimal PortionAmount { get; set; }
	}
}