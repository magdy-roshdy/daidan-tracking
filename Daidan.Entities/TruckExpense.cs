using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daidan.Entities
{
	public class TruckExpense
	{
		virtual public long Id { get; set; }
		virtual public decimal Amount { get; set; }
		virtual public int Month { get; set; }
		virtual public int Year { get; set; }

		virtual public Truck Truck { get; set; }
		virtual public Driver Driver { get; set; }
		virtual public ExpensesSection Section { get; set; }
	}
}
