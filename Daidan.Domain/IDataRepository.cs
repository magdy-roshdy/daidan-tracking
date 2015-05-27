using Daidan.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daidan.Domain
{
	public interface IDataRepository
	{
		Trip SaveTrip(Trip trip);
		IList<Trip> GetAddedTodayTrips();

		IList<Driver> GetAllDrivers();
		IList<Customer> GetAllCustomers();
		IList<Truck> GetAllTrucks();
		IList<Unit> GetAllUnits();
		IList<Material> GetAllMaterials();
		IList<Site> GetAllSites();

		Trip GetTripById(long tripId);
		void DeleteTrip(long tripId);

		IList<Trip> MasterReportSearch(MasterReportSearchParameters parameters);

		Driver GetDriverById(int driverId);
		Driver SaveDriver(Driver driver);
		bool DeleteDriver(int driverId);

		Material GetMaterialById(int materialId);
		Material SaveMaterial(Material material);
		bool DeleteMaterial(int materialId);

		Customer GetCustomerById(int customerId);
		Customer SaveCustomer(Customer customer);
		bool DeleteCustomer(int customerId);

		IList<Site> GetSitesByCustomerId(int customerId);
		Site GetSiteById(int siteId);
		Site SaveSite(Site site);
		bool DeleteSite(int siteId);

		Truck GetTruckById(int truckId);
		IList<Driver> GetTruckFreeDrivers(bool? outsourced = null);
		Truck SaveTruck(Truck truck);
		bool DeleteTruck(int truckId);
		
		bool PONumberBatchUpdate(IList<long> tripsIds, string newPONumber, int systemAdminId);
		bool SellingPriceBatchUpdate(IList<long> tripsIds, decimal newSellingPrice, int systemAdminId);

		SystemAdmin GetSystemAdminByEmail(string email);
		IList<SystemAdmin> GetAllSystemAdmins();
		SystemAdmin GetSystemAdminById(int systemAdminId);
		SystemAdmin SaveSystemAdmin(SystemAdmin systemAdmin, bool includePassword);
		bool DeleteSystemAdmin(int systemAdminId);

		string[] GetUsersRoles(string email);
	}
}