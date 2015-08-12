using Daidan.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Daidan.Web.Models
{
	public class MonthAdminPercentageEditViewModel
	{
		public MonthAminPercentage Month { get; set; }
		public List<SelectListItem> Months { get; set; }
		public int? MonthToCopyFrom { get; set; }
		public List<SelectListItem> PreviouseMonths { get; set; }
	}
}