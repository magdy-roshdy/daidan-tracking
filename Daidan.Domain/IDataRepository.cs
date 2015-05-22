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
	}
}