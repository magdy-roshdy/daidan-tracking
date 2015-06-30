using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daidan.Entities
{
	public class CustomerPercentage
	{
		public virtual double Id { get; set; }
		public virtual MonthPercentage Month { get; set; }
		public virtual Customer Customer { get; set; }
		public virtual decimal Amount { get; set; }

		public virtual IList<MaterialPercentage> MaterialsPercentage { get; set; }
	}
}
