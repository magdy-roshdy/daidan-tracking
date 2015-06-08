using Daidan.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Daidan.Web.Models
{
	public class TrucksExpensesEditViewModel
	{
		public long ExpenseId { get; set; }
		public int TruckId { get; set; }
		public int? DriverId { get; set; }
		public int Month { get; set; }

		[Required(ErrorMessage="Please enter the year")]
		public int Year { get; set; }
		public int SectionId { get; set; }

		[Required(ErrorMessage = "Please enter the amount")]
		public decimal? Amount { get; set; }

		public List<SelectListItem> Months { get; set; }
		public List<SelectListItem> Trucks { get; set; }
		public List<SelectListItem> Sections { get; set; }
		public List<SelectListItem> Drivers { get; set; }
	}
}