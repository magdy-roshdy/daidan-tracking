using Daidan.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Daidan.Web.Models
{
	public class DriverSalariesListViewModel
	{
		public IList<DriverSalary> Salaries { get; set; }
		public Driver Driver { get; set; }
	}
}