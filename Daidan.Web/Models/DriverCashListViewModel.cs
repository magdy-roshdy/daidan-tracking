using Daidan.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Daidan.Web.Models
{
	public class DriverCashListViewModel
	{
		public string DriverName { get; set; }
		public int DriverId { get; set; }
		public DateTime From { get; set; }
		public DateTime To { get; set; }

		public IList<DriverCash> DriverCashList { get; set; }
	}
}