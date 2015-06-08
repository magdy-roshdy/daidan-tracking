using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daidan.Entities
{
	public class DriverCash
	{
		public virtual long Id { get; set; }
		public virtual Driver Driver { get; set; }
		public virtual decimal Amount { get; set; }
		public virtual DateTime Date { get; set; }
		public virtual string VoucherNumber { get; set; }
	}
}
