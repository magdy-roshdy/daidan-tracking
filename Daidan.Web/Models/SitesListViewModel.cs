using Daidan.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Daidan.Web.Models
{
	public class SitesListViewModel
	{
		public SitesListViewModel()
		{
			this.Sites = new List<Site>();
		}

		public IList<Site> Sites { get; set; }
		public Customer Customer { get; set; }
	}
}