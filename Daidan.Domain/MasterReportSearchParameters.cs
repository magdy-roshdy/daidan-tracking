using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Daidan.Domain
{
	public class MasterReportSearchParameters
	{
		public string VoucherNumber { get; set; }
		public string PONumber { get; set; }
		public DateTime? From { get; set; }
		public DateTime? To { get; set; }
		public int? DriverId { get; set; }
		public int? TruckId { get; set; }
		public int? CustomerId { get; set; }
		public int? SiteId { get; set; }
		public int? MaterialId { get; set; }
		public int? UnitId { get; set; }
	}
}