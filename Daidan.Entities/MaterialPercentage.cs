using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daidan.Entities
{
	public class MaterialPercentage
	{
		public virtual long Id { get; set; }
		public virtual CustomerPercentage CustomerPercentage { get; set; }
		public virtual Material Material { get; set; }
		public virtual decimal Amount { get; set; }
		public virtual bool IsAmount { get; set; }
	}
}
