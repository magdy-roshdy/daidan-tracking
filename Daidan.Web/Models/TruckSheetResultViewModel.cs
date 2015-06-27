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
	}
}