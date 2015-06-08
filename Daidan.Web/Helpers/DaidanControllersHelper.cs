using Daidan.Domain;
using Daidan.Entities;
using Daidan.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Daidan.Web.Helpers
{
	public class DaidanControllersHelper
	{
		public static AddTripLookup GetTripLookups(IDataRepository dbRepository)
		{
			AddTripLookup lookups = new AddTripLookup();
			lookups.Customers = dbRepository.GetAllCustomers();
			lookups.Drivers = dbRepository.GetAllDrivers();
			lookups.Trucks = dbRepository.GetAllTrucks();
			lookups.Materials = dbRepository.GetAllMaterials();
			lookups.Units = dbRepository.GetAllUnits();
			lookups.Sites = dbRepository.GetAllSites();
			lookups.LastInsertionDate = DateTime.Now;

			return lookups;
		}

		public static List<SelectListItem> CustomersToSelectListItems(IList<Customer> customers, int selectedId = -1, bool addEmptyItem = false)
		{
			List<SelectListItem> selectListItems = new List<SelectListItem>();

			if (addEmptyItem)
			{
				selectListItems.Add(new SelectListItem { Text = "", Value = "0" });
			}

			foreach (Customer customer in customers)
			{
				selectListItems.Add(new SelectListItem { Text = customer.Name, Value = customer.Id.ToString(), Selected = (selectedId == customer.Id) });
			}

			return selectListItems;
		}

		public static List<SelectListItem> DriversToSelectListItems(IList<Driver> drivers, int selectedId = -1, bool addEmptyItem = false)
		{
			List<SelectListItem> selectListItems = new List<SelectListItem>();

			if (addEmptyItem)
			{
				selectListItems.Add(new SelectListItem { Text = "", Value = "0" });
			}

			foreach (Driver driver in drivers)
			{
				selectListItems.Add(new SelectListItem { Text = driver.Name, Value = driver.Id.ToString(), Selected = (selectedId == driver.Id) });
			}

			return selectListItems;
		}

		public static string ComputePasswordHash(string plainText, byte[] saltBytes = null, string hashAlgorithm = "")
		{
			if (saltBytes == null)
			{
				int saltSize = new Random().Next(4, 8);
				saltBytes = new byte[saltSize];
				new RNGCryptoServiceProvider().GetNonZeroBytes(saltBytes);
			}

			byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
			byte[] plainTextWithSaltBytes = new byte[plainTextBytes.Length + saltBytes.Length];

			for (int i = 0; i < plainTextBytes.Length; i++)
				plainTextWithSaltBytes[i] = plainTextBytes[i];

			// Append salt bytes to the resulting array.
			for (int i = 0; i < saltBytes.Length; i++)
				plainTextWithSaltBytes[plainTextBytes.Length + i] = saltBytes[i];

			HashAlgorithm hash = null;

			switch (hashAlgorithm.ToUpper())
			{
				case "SHA384":
					hash = new SHA384Managed();
					break;
				case "SHA512":
					hash = new SHA512Managed();
					break;
				default:
					hash = new MD5CryptoServiceProvider();
					break;
			}

			byte[] hashBytes = hash.ComputeHash(plainTextWithSaltBytes);
			byte[] hashWithSaltBytes = new byte[hashBytes.Length + saltBytes.Length];
			for (int i = 0; i < hashBytes.Length; i++)
				hashWithSaltBytes[i] = hashBytes[i];

			// Append salt bytes to the result.
			for (int i = 0; i < saltBytes.Length; i++)
				hashWithSaltBytes[hashBytes.Length + i] = saltBytes[i];

			return Convert.ToBase64String(hashWithSaltBytes);
		}

		public static bool VerifyPasswordHash(string plainText, string hashValue, string hashAlgorithm = "")
		{
			byte[] hashWithSaltBytes = Convert.FromBase64String(hashValue);

			int hashSize;

			switch (hashAlgorithm.ToUpper())
			{

				case "SHA384":
					hashSize = 384 / 8;
					break;

				case "SHA512":
					hashSize = 512 / 8;
					break;

				default: //MD5
					hashSize = 128 / 8;
					break;
			}

			if (hashWithSaltBytes.Length < hashSize)
				return false;

			byte[] saltBytes = new byte[hashWithSaltBytes.Length - hashSize];

			// Copy salt from the end of the hash to the new array.
			for (int i = 0; i < saltBytes.Length; i++)
				saltBytes[i] = hashWithSaltBytes[hashSize + i];

			string expectedHashString = ComputePasswordHash(plainText, saltBytes, hashAlgorithm);
			return (hashValue == expectedHashString);
		}

		public static List<SelectListItem> RolesListToSelectitems(string selected, bool addEmptyItem = false)
		{
			string[] roles = new string[] {"data-entry", "admin", "systemAdmin"};
			List<SelectListItem> rolesSelectItems = new List<SelectListItem>();
			if (addEmptyItem)
			{
				rolesSelectItems.Add(new SelectListItem { Text = "", Value = "" });
			}

			foreach(string role in roles)
			{
				rolesSelectItems.Add(new SelectListItem { Text = role, Value = role, Selected = (role == selected) });
			}

			return rolesSelectItems;
		}

		public static SystemAdmin IdentityUserToSystemAdmin(System.Security.Principal.IIdentity identity)
		{
			SystemAdmin systemAdmin = null;
			ClaimsIdentity claimIdentity = identity as ClaimsIdentity;
			if(claimIdentity != null)
			{
				systemAdmin = new SystemAdmin();
				systemAdmin.Id = int.Parse(claimIdentity.FindFirst(ClaimTypes.Sid).Value);
				systemAdmin.Email = claimIdentity.FindFirst(ClaimTypes.Email).Value;
				systemAdmin.Name = claimIdentity.FindFirst(ClaimTypes.GivenName).Value;
				systemAdmin.Role = claimIdentity.FindFirst(ClaimTypes.Role).Value;
			}

			return systemAdmin;
		}

		public static bool IsUserAdmin(System.Security.Principal.IIdentity identity)
		{
			SystemAdmin admin = DaidanControllersHelper.IdentityUserToSystemAdmin(identity);
			return (admin != null && admin.Role == "admin");
		}

		public static bool IsUserSystemAdmin(System.Security.Principal.IIdentity identity)
		{
			SystemAdmin admin = DaidanControllersHelper.IdentityUserToSystemAdmin(identity);
			return (admin != null && admin.Role == "systemAdmin");
		}

		public static List<SelectListItem> TrucksToSelectListItems(IList<Truck> trucks, int selectedId = -1, bool addEmptyItem = false)
		{
			List<SelectListItem> selectListItems = new List<SelectListItem>();

			if (addEmptyItem)
			{
				selectListItems.Add(new SelectListItem { Text = "", Value = "0" });
			}

			foreach (Truck truck in trucks)
			{
				selectListItems.Add(new SelectListItem { Text = truck.Number, Value = truck.Id.ToString(), Selected = (selectedId == truck.Id) });
			}

			return selectListItems;
		}

		public static List<SelectListItem> YearMonthsToSelectListItems(int selectedId = -1, bool addEmptyItem = false)
		{
			List<SelectListItem> selectListItems = new List<SelectListItem>();

			if (addEmptyItem)
			{
				selectListItems.Add(new SelectListItem { Text = "", Value = "0" });
			}

			DateTime date;
			for (int month = 1; month <= 12; month++)
			{
				date = new DateTime(1, month, 1);
				selectListItems.Add(new SelectListItem { Text = date.ToString("MMMM"), Value = date.Month.ToString(), Selected = (selectedId == month) });
			}

			return selectListItems;
		}

		public static List<SelectListItem> TrucksExpenseSectionsToSelectListItems(IList<ExpensesSection> expenseSections, int selectedId = -1, bool addEmptyItem = false)
		{
			List<SelectListItem> selectListItems = new List<SelectListItem>();

			if (addEmptyItem)
			{
				selectListItems.Add(new SelectListItem { Text = "", Value = "0" });
			}

			foreach (ExpensesSection section in expenseSections)
			{
				selectListItems.Add(new SelectListItem { Text = section.Name, Value = section.Id.ToString(), Selected = (selectedId == section.Id) });
			}

			return selectListItems;
		}
	}
}