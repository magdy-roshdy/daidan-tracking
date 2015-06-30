using Daidan.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Daidan.Web.Models
{
	public class DriverSalaryEditViewModel
	{
		public long DriverSalaryId { get; set; }
		public int Month { get; set; }

		[Required(ErrorMessage = "Please enter the year")]
		public int Year { get; set; }
		
		[Required(ErrorMessage = "Please enter the salary amount")]
		public decimal Amount { get; set; }

		public int DriverId { get; set; }
		public Driver Driver { get; set; }

		public List<SelectListItem> Months { get; set; }
	}
}