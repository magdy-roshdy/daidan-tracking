using Daidan.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Daidan.Web.Models
{
	public class AdminPercentageEditViewModel
	{
		public IList<MaterialAdminPercentage> MaterialPercentages { get; set; }
		public IList<Material> MaterialsList { get; set; }
		public IList<Customer> CustomersList { get; set; }
	}
}