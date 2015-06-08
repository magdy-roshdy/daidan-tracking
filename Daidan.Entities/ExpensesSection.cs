using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daidan.Entities
{
	public class ExpensesSection
	{
		virtual public int Id { get; set; }
		virtual public string Name { get; set; }
		virtual public bool IsActive { get; set; }
	}
}
