using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daidan.Entities
{
	public class MaterialAdminPercentage
	{
		public virtual long Id { get; set; }
		public virtual MonthAminPercentage Month { get; set; }
		public virtual Material Material { get; set; }
		public virtual decimal Amount { get; set; }
		public virtual bool IsFixedAmount { get; set; }

		public virtual IList<CustomerAdminPercentage> CustomersPercentage { get; set; }
	}
}
