using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daidan.Entities
{
	public class Customer
	{
		public virtual int Id { get; set; }
		[Required(ErrorMessage = "Please enter customer name")]
		public virtual string Name { get; set; }
		public virtual bool IsActive { get; set; }
	}
}
