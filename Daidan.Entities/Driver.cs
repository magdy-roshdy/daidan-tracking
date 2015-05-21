using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Daidan.Entities
{
	public class Driver
	{
		public virtual int Id { get; set; }
		[Required(ErrorMessage="Please enter driver name")]
		public virtual string Name { get; set; }
		public virtual bool IsActive { get; set; }
		public virtual bool IsOutsourced { get; set; }
	}
}
