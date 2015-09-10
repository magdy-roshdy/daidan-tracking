using Daidan.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Daidan.Web.Models
{
	public class MonthSummaryByDriverViewModel
	{
		public DateTime Month { get; set; }
		public IList<SummaryItem> SummaryItems { get; set; }
		public ExpensesSection[] MonthExpensesSections { get; set; }

		public class SummaryItem
		{
			public Driver Driver { get; set; }
			public int NofTrips { get; set; }
			public decimal GrossProfit { get; set; }
			public TruckExpense[] PaidTruckExpenses { get; set; }
			public decimal Salary { get; set; }
			public decimal DriversCommission
			{
				get
				{
					if ((this.GrossProfit - this.AdminFees) > 0)
						return (this.GrossProfit - this.AdminFees) * 0.1M;
					else
						return 0;
				}
			}
			public decimal RecievedCash { get; set; }
			public decimal AdminFees { get; set; }
			public decimal Result
			{
				get
				{
					return this.GrossProfit - (this.PaidTruckExpenses.Sum(x => x.Amount) + this.Salary + this.DriversCommission + this.AdminFees);
				}
			}

			public decimal ResultPlusAdmin
			{
				get
				{
					return this.Result + this.AdminFees;
				}
			}
		}
	}
}