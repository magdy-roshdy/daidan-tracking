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

		IList<TruckExpense> GetTruckExpenses(int truckId, int? month = null, int? year = null);
		IList<ExpensesSection> GetAllTrucksExpensesSections();
		TruckExpense SaveTruckExpense(TruckExpense expense);
		TruckExpense GetTruckExpenseById(long expenseId);
		bool DeleteTruckExpense(long expenseId);

		IList<DriverCash> GetDriverCashList(int driverId, DateTime? from = null, DateTime? to = null);
		DriverCash SaveDriverCash(DriverCash cash);
		DriverCash GetDriverCashById(long cashId);
		bool DeleteDriverCash(long cashId);

		IList<TruckExpense> GetTruckSheetExpenses(int truckId, int month, int year);

		IList<MonthPercentage> GetMonthsPercentage(int month, int year);

		IList<DriverSalary> GetDriverSalaries(int driverId);
		DriverSalary GetDriverSalaryById(long driverSalaryId);
		DriverSalary SaveDriverSalary(DriverSalary salary);
		bool DeleteDriverSalary(long salaryId);
		DriverSalary GetDriverMonthSalary(int driverId, int month, int year);

		IList<TruckExpense> GetTruckExpensesByDriver(int driverId, int month, int year);
		
		DriverMonthBalance GetDriverMonthBalance(int driverId, int month, int year);

		bool UpdateDriverMonthBalanace(int driverId, int month, int year, decimal balanace);
	}
}