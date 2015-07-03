using Daidan.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Daidan.Web.Models
{
	public class DriverSheetResultViewModel
	{
		public DriverSheetResultViewModel()
		{
			this.MonthPaidExpenses = new List<TruckExpense>();
		}

		public IList<Trip> Trips { get; set; }

		public decimal PreviousMonthBalanace { get; set; }
		public decimal MonthSalary { get; set; }
		public decimal DriverTripsProfit { get; set; }
		public decimal MonthPaidExpensesSum { get; set; }
		public decimal ExtraCostSum { get; set; }
		public decimal MonthDriverCashSum { get; set; }

		public IList<DriverCash> MonthDriverCash { get; set; }
		public IList<TruckExpense> MonthPaidExpenses { get; set; }

		public decimal MonthBalance
		{
			get
			{
				return PreviousMonthBalanace + MonthSalary
					+ DriverTripsProfit - MonthDriverCashSum
					+ ExtraCostSum + MonthPaidExpensesSum;
			}
		}
	}
}