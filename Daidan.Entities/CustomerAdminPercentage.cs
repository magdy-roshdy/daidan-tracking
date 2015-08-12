using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Daidan.Entities
{
	public class CustomerAdminPercentage
	{
		public virtual long Id { get; set; }
		public virtual Customer Customer { get; set; }
		[ScriptIgnore]
		public virtual MaterialAdminPercentage MaterialPercentage { get; set; }
		public virtual decimal Amount { get; set; }
		public virtual bool IsFixedAmount { get; set; }
	}
}
