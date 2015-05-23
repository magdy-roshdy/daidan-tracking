using Daidan.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Daidan.Web.Models
{
	public class EditSiteViewModel
	{
		public EditSiteViewModel()
		{
			this.Customers = new List<Customer>();
		}		

		public int SiteId { get; set; }

		[Required(ErrorMessage = "Please enter site name")]
		public string SiteName { get; set; }
		public bool SiteIsActive { get; set; }
		public bool SiteIsOwnSite { get; set; }

		public int CustomerId { get; set; }

		public IList<Customer> Customers { get; set; }
	}
}