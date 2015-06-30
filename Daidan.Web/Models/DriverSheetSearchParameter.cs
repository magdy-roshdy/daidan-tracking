using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Daidan.Web.Models
{
	public class DriverSheetSearchParameter
	{
		public int DriverId { get; set; }
		public int Month { get; set; }
		public int Year { get; set; }
	}
}