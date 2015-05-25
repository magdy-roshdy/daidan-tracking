using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Daidan.Web.Models
{
	public class LoginViewModel
	{
		[Required(ErrorMessage = "Please enter the email")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Please enter the password")]
		public string Password { get; set; }
	}
}