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
			public decimal Profit { get; set; }
			public decimal Expenses { get; set; }
			public decimal Result {
				get
				{
					return Profit - Expenses;
				}
			}
		}
	}
}