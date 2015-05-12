using Daidan.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Daidan.Web.Models
{
	public class AddTripViewModel
	{
		public IList<Trip> TripsList { get; set; }
		public AddTripLookup AddTripLookup { get; set; }
	}
}