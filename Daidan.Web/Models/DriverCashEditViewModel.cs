using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Daidan.Web.Models
{
	public class DriverCashEditViewModel
	{
		public long DriverCashId { get; set; }
		public int DriverId { get; set; }

		[Required(ErrorMessage="Please enter cash amount")]
		public decimal? Amount { get; set; }

		[Required(ErrorMessage = "Please enter cash date")]
		public DateTime? Date { get; set; }

		public string VoucherNumber { get; set; }
		public IList<SelectListItem> Drivers { get; set; }
	}
}