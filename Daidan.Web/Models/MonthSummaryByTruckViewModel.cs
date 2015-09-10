using Daidan.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Daidan.Web.Models
{
	public class MonthSummaryByTruckViewModel
	{
		public DateTime Month { get; set; }
		public IList<SummaryItem> SummaryItems { get; set; }

		public class SummaryItem
		{
			public Truck Truck { get; set; }
			public int NofTrips { get; set; }
			public decimal GrossProfit { get; set; }
			public decimal NetProfit {get; set; }
			public decimal Expenses { get; set; }
			public decimal DriversBasicSalary { get; set; }
			public decimal AdminFees { get; set; }
			public decimal Result {
				get
				{
					return this.GrossProfit - (this.Expenses + this.DriversBasicSalary + this.AdminFees);
				}
			}
			public decimal DriversSalary
			{
				get
				{
					return this.DriversBasicSalary + this.DriversCommission;
				}
			}

			private decimal DriversCommission
			{
				get
				{
					return this.NetProfit * 0.1M;
				}
			}
		}
	}
}