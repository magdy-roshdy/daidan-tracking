using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daidan.Entities
{
	public class DriverMonthBalance
	{
		public virtual long Id { get; set; }

		public virtual int Month { get; set; }
		public virtual int Year { get; set; }
		public virtual decimal Amount { get; set; }
		public virtual DateTime LastModefied { get; set; }

		public virtual Driver Driver { get; set; }
	}
}
