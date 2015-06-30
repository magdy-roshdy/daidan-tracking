using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daidan.Entities
{
	public class MonthPercentage
	{
		public virtual int Id { get; set; }
		public virtual int Month { get; set; }
		public virtual int Year { get; set; }
		public virtual decimal Amount { get; set; }

		public virtual IList<CustomerPercentage> CustomersPercentage { get; set; }
	}
}
