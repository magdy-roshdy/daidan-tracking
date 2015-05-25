using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daidan.Entities
{
	public class SystemAdmin
	{
		virtual public int Id { get; set; }
		virtual public string Name { get; set; }

		[Required(ErrorMessage = "Please enter the email")]
		virtual public string Email { get; set; }

		[Required(ErrorMessage = "Please enter the password")]
		virtual public string Password { get; set; }

		virtual public bool IsActive { get; set; }
		virtual public string Role { get; set; }
	}
}
