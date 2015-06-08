using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Daidan.Web.Models
{
	public class DriversCashIndexViewModel
	{
		public int DriverId { get; set; }

		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
		[Required(ErrorMessage="Please enter from date")]
		public DateTime? From { get; set; }

		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
		[Required(ErrorMessage = "Please enter to date")]
		public DateTime? To { get; set; }
		public List<SelectListItem> Drivers { get; set; }
	}
}