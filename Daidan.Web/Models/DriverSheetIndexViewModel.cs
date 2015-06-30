using Daidan.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Daidan.Web.Models
{
	public class DriverSheetIndexViewModel
	{
		public List<SelectListItem> Drivers { get; set; }
		public List<SelectListItem> Months { get; set; }

		public int Month { get; set; }
		public int Year { get; set; }

		public int DriverId { get; set; }
	}
}