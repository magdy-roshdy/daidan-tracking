using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daidan.Entities
{
	public class Unit
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual bool IsActive { get; set; }
		public virtual bool IsOneQuantityOnly { get; set; }
	}
}
