using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daidan.Entities
{
	public class Site
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual bool IsActive { get; set; }
		public virtual bool IsOwnSite { get; set; }

		public virtual Customer Customer { get; set; }
	}
}
