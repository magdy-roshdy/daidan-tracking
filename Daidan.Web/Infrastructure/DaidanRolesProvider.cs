using Daidan.Domain;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.ClientServices.Providers;
using System.Web.Security;

namespace Daidan.Web.Infrastructure
{
	public class DaidanRolesProvider : ClientRoleProvider
	{
		[Inject]
		public IDataRepository DBRepository { get; set; }
		
		public override string[] GetRolesForUser(string username)
		{
			return this.DBRepository.GetUsersRoles(username);
		}

		public override bool IsUserInRole(string username, string roleName)
		{
			return !string.IsNullOrEmpty(this.GetRolesForUser(username).FirstOrDefault(x => x == roleName));
		}
	}
}