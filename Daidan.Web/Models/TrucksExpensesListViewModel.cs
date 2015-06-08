using Daidan.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Daidan.Web.Models
{
	public class TrucksExpensesListViewModel
	{
		public string TruckNumber { get; set; }
		public int TruckId { get; set; }
		public string Month { get; set; }
		public int Year { get; set; }

		public IList<TruckExpense> TruckExpenseList { get; set; }
	}
}