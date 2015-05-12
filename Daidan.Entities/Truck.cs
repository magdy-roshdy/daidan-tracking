using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daidan.Entities
{
	public class Truck
	{
		public virtual int Id { get; set; }
		public virtual string Number { get; set; }
		public virtual bool IsActive { get; set; }
		public virtual bool IsOutsourced { get; set; }

		public virtual Driver Driver { get; set; }
	}
}
