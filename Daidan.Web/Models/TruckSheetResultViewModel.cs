using Daidan.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Daidan.Web.Models
{
	public class TruckSheetResultViewModel
	{
		public IList<Trip> Trips { get; set; }
		public IList<TruckExpense> Expenses { get; set; }
		public IList<TripsDriverSalaryPortion> DriversSalaryPortion { get; set; }

		public decimal MonthAdminFeesSum
		{
			get
			{
				return this.Trips.Sum(x => x.AdminFeesAmount);
			}
		}
		public decimal DriversSalaryPortionSum
		{
			get
			{
				return this.DriversSalaryPortion.Sum(x => x.PortionAmount);
			}
		}
		public decimal DriversSalaryPortionTripsCount
		{
			get
			{
				return this.DriversSalaryPortion.Sum(x => x.NumberOfTrips);
			}
		}
	}
}