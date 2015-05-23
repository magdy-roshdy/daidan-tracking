using Daidan.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Daidan.Web.Models
{
	public class EditTruckViewModel
	{
		public EditTruckViewModel()
		{
			this.Drivers = new List<SelectListItem>();
		}

		public int TruckId  { get; set; }
		[Required(ErrorMessage = "Please enter truck number")]
		public string TruckNumber { get; set; }
		public bool TruckIsActive { get; set; }
		public bool TruckIsOutsourced { get; set; }
		public bool TruckIsOutsourcedX
		{
			get { return this.TruckIsOutsourced; }
			set { }
		}
		
		public int TruckDriverId { get; set; }

		public List<SelectListItem> Drivers { get; set; }
	}
}