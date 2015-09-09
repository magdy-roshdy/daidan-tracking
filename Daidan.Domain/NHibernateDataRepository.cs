using Daidan.Entities;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daidan.Domain
{
	public class NHibernateDataRepository : IDataRepository
	{
		public ISessionFactory SessionFactory
		{
			set;
			private get;
		}

		public Trip SaveTrip(Trip trip)
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					if (trip.Id > 0)
					{
						Trip db_trip = session.Get<Trip>(trip.Id);

						db_trip.Driver = session.Get<Driver>(trip.Driver.Id);
						db_trip.Truck = session.Get<Truck>(trip.Truck.Id);
						db_trip.Material = session.Get<Material>(trip.Material.Id);
						db_trip.Site = session.Get<Site>(trip.Site.Id);
						db_trip.Site.Customer = session.Get<Customer>(trip.Site.Customer.Id);
						db_trip.Unit = session.Get<Unit>(trip.Unit.Id);

						db_trip.Date = trip.Date;
						db_trip.VoucherNumber = trip.VoucherNumber;
						db_trip.PONumber = trip.PONumber ?? null;
						db_trip.TicketNumber = trip.TicketNumber ?? null;
						db_trip.UnitCost = trip.UnitCost;
						db_trip.UnitSellingPrice = trip.UnitSellingPrice;
						db_trip.UnitsQuantity = trip.UnitsQuantity;
						db_trip.ExtraCost = trip.ExtraCost;
						db_trip.LastModifiedOn = DateTime.Now;
						db_trip.LastModefiedBy = session.Get<SystemAdmin>(trip.LastModefiedBy.Id);

						session.Save(db_trip);
						trip = db_trip;
					}
					else
					{
						trip.Driver = session.Get<Driver>(trip.Driver.Id);
						trip.Truck = session.Get<Truck>(trip.Truck.Id);
						trip.Material = session.Get<Material>(trip.Material.Id);
						trip.Site = session.Get<Site>(trip.Site.Id);
						trip.Site.Customer = session.Get<Customer>(trip.Site.Customer.Id);
						trip.Unit = session.Get<Unit>(trip.Unit.Id);

						trip.Date = trip.Date;
						trip.VoucherNumber = trip.VoucherNumber;
						trip.PONumber = trip.PONumber ?? null;
						trip.TicketNumber = trip.TicketNumber ?? null;
						trip.UnitCost = trip.UnitCost;
						trip.UnitSellingPrice = trip.UnitSellingPrice;
						trip.UnitsQuantity = trip.UnitsQuantity;
						trip.ExtraCost = trip.ExtraCost;
						trip.AddedOn = DateTime.Now;
						trip.AddedBy = session.Get<SystemAdmin>(trip.AddedBy.Id);

						session.Save(trip);
					}

					transaction.Commit();
					session.Flush();
				}
			}

			return trip;
		}

		public IList<Trip> GetAddedTodayTrips()
		{
			using(ISession session = SessionFactory.OpenSession())
			{
				return session.QueryOver<Trip>().Where
					(Restrictions.Eq(
						Projections.SqlFunction("year"
						,NHibernateUtil.Int32
						, Projections.Property<Trip>(t => t.AddedOn))
						,DateTime.Now.Year)
					).Where
					(
						Restrictions.Eq(
						Projections.SqlFunction("month"
						,NHibernateUtil.Int32
						,Projections.Property<Trip>(t => t.AddedOn))
						,DateTime.Now.Month)
					)
					.Where
					(
						Restrictions.Eq(
						Projections.SqlFunction("day"
						, NHibernateUtil.Int32
						, Projections.Property<Trip>(t => t.AddedOn))
						, DateTime.Now.Day)
					)
					.OrderBy(x => x.Date).Desc
					.List();
			}
		}

		public IList<Driver> GetAllDrivers()
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				return session.QueryOver<Driver>().List();
			}
		}

		public IList<Customer> GetAllCustomers()
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				return session.QueryOver<Customer>().List();
			}
		}

		public IList<Truck> GetAllTrucks()
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				return session.QueryOver<Truck>().List();
			}
		}

		public IList<Unit> GetAllUnits()
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				return session.QueryOver<Unit>().List();
			}
		}

		public IList<Material> GetAllMaterials()
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				return session.QueryOver<Material>().List();
			}
		}

		public IList<Site> GetAllSites()
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				return session.QueryOver<Site>().List();
			}
		}

		public Trip GetTripById(long tripId)
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				return session.Get<Trip>(tripId);
			}
		}

		public void DeleteTrip(long tripId)
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					Trip db_trip = session.Get<Trip>(tripId);
					session.Delete(db_trip);
					transaction.Commit();
					session.Flush();
				}
			}
		}

		public IList<Trip> MasterReportSearch(MasterReportSearchParameters parameters)
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				IQueryOver<Trip, Trip> queryOver = session.QueryOver<Trip>();

				if (parameters.DriverId.HasValue)
					queryOver.Where(x => x.Driver.Id == parameters.DriverId);
				
				if (parameters.CustomerId.HasValue) 
					queryOver.Inner.JoinQueryOver<Site>(y => y.Site).Where(z => z.Customer.Id == parameters.CustomerId);
				
				if (parameters.SiteId.HasValue)
					queryOver.Where(x => x.Site.Id == parameters.SiteId);
				if (parameters.MaterialId.HasValue)
					queryOver.Where(x => x.Material.Id == parameters.MaterialId);
				if (parameters.UnitId.HasValue)
					queryOver.Where(x => x.Unit.Id == parameters.UnitId);
				if (parameters.TruckId.HasValue)
					queryOver.Where(x => x.Truck.Id == parameters.TruckId);

				if (!string.IsNullOrWhiteSpace(parameters.VoucherNumber))
					queryOver.Where(x => x.VoucherNumber == parameters.VoucherNumber);
				if (!string.IsNullOrWhiteSpace(parameters.PONumber))
					queryOver.Where(x => x.PONumber == parameters.PONumber);
				if (!string.IsNullOrWhiteSpace(parameters.TicketNumber))
					queryOver.Where(x => x.TicketNumber == parameters.TicketNumber);
				
				if (parameters.From.HasValue)
					queryOver.Where(x => x.Date >= parameters.From.Value);
					
				if (parameters.To.HasValue)
					queryOver.Where(x => x.Date <= parameters.To.Value);

				return queryOver.List();
			}
		}

		public Driver GetDriverById(int driverId)
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				return session.Get<Driver>(driverId);
			}
		}

		public Driver SaveDriver(Driver driver)
		{
			Driver db_driver = null;
			try
			{
				using (ISession session = SessionFactory.OpenSession())
				{
					using (ITransaction transaction = session.BeginTransaction())
					{
						if (driver.Id > 0)
						{
							db_driver = session.Get<Driver>(driver.Id);

							db_driver.Name = driver.Name;
							db_driver.IsActive = driver.IsActive;
							db_driver.IsOutsourced = driver.IsOutsourced;


							session.Save(db_driver);
							driver = db_driver;
						}
						else
						{
							session.Save(driver);
							db_driver = driver;
						}

						transaction.Commit();
						session.Flush();
					}
				}
			}
			catch
			{
				db_driver = null;
			}

			return db_driver;
		}

		public bool DeleteDriver(int driverId)
		{
			bool result = false;
			using (ISession session = SessionFactory.OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					try
					{
						Driver db_driver = session.Get<Driver>(driverId);
						session.Delete(db_driver);
						transaction.Commit();
						session.Flush();

						result = true;
					}
					catch
					{
						result = false;
					}
					
				}
			}

			return result;
		}

		public Material GetMaterialById(int materialId)
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				return session.Get<Material>(materialId);
			}
		}

		public Material SaveMaterial(Material material)
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					if (material.Id > 0)
					{
						Material db_material = session.Get<Material>(material.Id);

						db_material.Name = material.Name;
						db_material.IsActive = material.IsActive;

						session.Save(db_material);
						material = db_material;
					}
					else
					{
						session.Save(material);
					}

					transaction.Commit();
					session.Flush();
				}
			}

			return material;
		}

		public bool DeleteMaterial(int materialId)
		{
			bool result = false;
			using (ISession session = SessionFactory.OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					try
					{
						Material db_material = session.Get<Material>(materialId);
						session.Delete(db_material);
						transaction.Commit();
						session.Flush();

						result = true;
					}
					catch
					{
						result = false;
					}

				}
			}

			return result;
		}

		public Customer GetCustomerById(int customerId)
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				return session.Get<Customer>(customerId);
			}
		}

		public Customer SaveCustomer(Customer customer)
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					if (customer.Id > 0)
					{
						Customer db_customer = session.Get<Customer>(customer.Id);

						db_customer.Name = customer.Name;
						db_customer.IsActive = customer.IsActive;

						session.Save(db_customer);
						customer = db_customer;
					}
					else
					{
						session.Save(customer);
					}

					transaction.Commit();
					session.Flush();
				}
			}

			return customer;
		}

		public bool DeleteCustomer(int customerId)
		{
			bool result = false;
			using (ISession session = SessionFactory.OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					try
					{
						Customer db_customer = session.Get<Customer>(customerId);
						session.Delete(db_customer);
						transaction.Commit();
						session.Flush();

						result = true;
					}
					catch
					{
						result = false;
					}

				}
			}

			return result;
		}

		public IList<Site> GetSitesByCustomerId(int customerId)
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				return session.QueryOver<Site>().Where(x => x.Customer.Id == customerId).List();
			}
		}

		public Site GetSiteById(int siteId)
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				return session.Get<Site>(siteId);
			}
		}

		public Site SaveSite(Site site)
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					if (site.Id > 0)
					{
						Site db_site = session.Get<Site>(site.Id);

						db_site.Name = site.Name;
						db_site.IsActive = site.IsActive;
						db_site.IsOwnSite = site.IsOwnSite;
						db_site.Customer = session.Get<Customer>(site.Customer.Id);

						session.Save(db_site);
						site = db_site;
					}
					else
					{
						session.Save(site);
					}

					transaction.Commit();
					session.Flush();
				}
			}

			return site;
		}

		public bool DeleteSite(int siteId)
		{
			bool result = false;
			using (ISession session = SessionFactory.OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					try
					{
						Site db_site = session.Get<Site>(siteId);
						session.Delete(db_site);
						transaction.Commit();
						session.Flush();

						result = true;
					}
					catch
					{
						result = false;
					}

				}
			}

			return result;
		}

		public Truck GetTruckById(int truckId)
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				return session.Get<Truck>(truckId);
			}
		}

		public IList<Driver> GetTruckFreeDrivers(bool? outsourced = null)
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				string sql = "SELECT DriverId, DriverName, DriverIsActive, DriverIsOutsourced FROM Drivers LEFT JOIN Trucks ON TruckDriverId = DriverId WHERE TruckDriverId IS NULL AND DriverIsOutsourced = 0;";
				var drivers = session.CreateSQLQuery(sql).List();
				IList<Driver> driversList = new List<Driver>();
				foreach (object driverObj in drivers)
				{
					object[] properties = driverObj as object[];
					if(properties != null)
					{ 
						Driver driver = new Driver();
						driver.Id = (int)properties[0];
						driver.Name = properties[1].ToString();
						driver.IsActive = (bool)properties[2];
						driver.IsOutsourced = (bool)properties[3];
						driversList.Add(driver);
					}
				}

				return driversList;
			}
		}

		public Truck SaveTruck(Truck truck)
		{
			Truck db_truck = null;
			try
			{
				using (ISession session = SessionFactory.OpenSession())
				{
					using (ITransaction transaction = session.BeginTransaction())
					{
						if (truck.Id > 0)
						{
							db_truck = session.Get<Truck>(truck.Id);

							db_truck.Number = truck.Number;
							db_truck.IsActive = truck.IsActive;
							db_truck.IsOutsourced = truck.IsOutsourced;
							if (truck.Driver != null && truck.Driver.Id > 0)
								db_truck.Driver = session.Get<Driver>(truck.Driver.Id);
							else
								db_truck.Driver = null;

							session.Save(db_truck);
							truck = db_truck;
						}
						else
						{
							session.Save(truck);
							db_truck = truck;
						}

						transaction.Commit();
						session.Flush();
					}
				}
			}
			catch
			{
				db_truck = null;
			}

			return db_truck;
		}

		public bool DeleteTruck(int truckId)
		{
			bool result = false;
			using (ISession session = SessionFactory.OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					try
					{
						Truck db_truck = session.Get<Truck>(truckId);
						session.Delete(db_truck);
						transaction.Commit();
						session.Flush();

						result = true;
					}
					catch
					{
						result = false;
					}

				}
			}

			return result;
		}

		public bool PONumberBatchUpdate(IList<long> tripsIds, string newPONumber, int systemAdminId)
		{
			bool result = false;
			using (ISession session = SessionFactory.OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					try
					{
						foreach(long id in tripsIds)
						{
							Trip t = session.Get<Trip>(id);
							if(t != null)
							{
								t.PONumber = newPONumber;
								t.LastModifiedOn = DateTime.Now;
								t.LastModefiedBy = session.Get<SystemAdmin>(systemAdminId);
								session.Save(t);
							}
						}

						transaction.Commit();
						session.Flush();

						result = true;
					}
					catch
					{
						result = false;
					}
				}
			}

			return result;
		}

		public bool SellingPriceBatchUpdate(IList<long> tripsIds, decimal newSellingPrice, int systemAdminId)
		{
			bool result = false;
			using (ISession session = SessionFactory.OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					try
					{
						foreach (long id in tripsIds)
						{
							Trip t = session.Get<Trip>(id);
							if (t != null)
							{
								t.UnitSellingPrice = newSellingPrice;
								t.LastModifiedOn = DateTime.Now;
								t.LastModefiedBy = session.Get<SystemAdmin>(systemAdminId);
								session.Save(t);
							}
						}

						transaction.Commit();
						session.Flush();

						result = true;
					}
					catch
					{
						result = false;
					}
				}
			}

			return result;
		}

		public SystemAdmin GetSystemAdminByEmail(string email)
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				return session.QueryOver<SystemAdmin>().Where(x => x.Email == email).List().FirstOrDefault();
			}
		}

		public IList<SystemAdmin> GetAllSystemAdmins()
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				return session.QueryOver<SystemAdmin>().Where(x => !x.IsBuiltIn).List();
			}
		}

		public SystemAdmin GetSystemAdminById(int systemAdminId)
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				return session.QueryOver<SystemAdmin>().Where(x => x.Id == systemAdminId && !x.IsBuiltIn).List().FirstOrDefault();
			}
		}

		public SystemAdmin SaveSystemAdmin(SystemAdmin systemAdmin, bool includePassword)
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					if (systemAdmin.Id > 0)
					{
						SystemAdmin db_systemAdmin = session.Get<SystemAdmin>(systemAdmin.Id);

						db_systemAdmin.Name = systemAdmin.Name;
						db_systemAdmin.Email = systemAdmin.Email;
						db_systemAdmin.Role = systemAdmin.Role;
						db_systemAdmin.IsActive = systemAdmin.IsActive;
						if (includePassword)
							db_systemAdmin.Password = systemAdmin.Password;

						session.Save(db_systemAdmin);
						systemAdmin = db_systemAdmin;
					}
					else
					{
						session.Save(systemAdmin);
					}

					transaction.Commit();
					session.Flush();
				}
			}

			return systemAdmin;
		}

		public bool DeleteSystemAdmin(int systemAdminId)
		{
			bool result = false;
			using (ISession session = SessionFactory.OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					try
					{
						SystemAdmin db_systemAdmin = session.QueryOver<SystemAdmin>().Where(x => x.Id == systemAdminId && !x.IsBuiltIn).SingleOrDefault();
						session.Delete(db_systemAdmin);
						transaction.Commit();
						session.Flush();

						result = true;
					}
					catch
					{
						result = false;
					}

				}
			}

			return result;
		}

		public string[] GetUsersRoles(string email)
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				List<string> roles = new List<string>();
				SystemAdmin admin = this.GetSystemAdminByEmail(email);
				if(admin != null)
				{
					roles.Add(admin.Role);
				}

				return roles.ToArray();
			}
		}

		public IList<TruckExpense> GetTruckExpenses(int truckId, int? month, int? year)
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				IQueryOver<TruckExpense, TruckExpense> queryOver = session.QueryOver<TruckExpense>();
				queryOver.Inner.JoinQueryOver<Truck>(y => y.Truck).Where(z => z.Id == truckId);

				if(month.HasValue && year.HasValue)
				{
					queryOver.Where(x => x.Month == month.Value);
					queryOver.Where(x => x.Year == year.Value);
				}

				return queryOver.List();
			}
		}

		public IList<ExpensesSection> GetAllTrucksExpensesSections()
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				return session.QueryOver<ExpensesSection>().List();
			}
		}

		public TruckExpense SaveTruckExpense(TruckExpense expense)
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					if (expense.Id > 0)
					{
						TruckExpense db_expense = session.Get<TruckExpense>(expense.Id);

						db_expense.Truck = session.Get<Truck>(expense.Truck.Id);
						db_expense.Month = expense.Month;
						db_expense.Year = expense.Year;
						db_expense.Section = session.Get<ExpensesSection>(expense.Section.Id);
						db_expense.Driver = expense.Driver != null ?  session.Get<Driver>(expense.Driver.Id) : null;
						db_expense.Amount = expense.Amount;
						
						session.Save(db_expense);
						expense = db_expense;
					}
					else
					{
						if (expense.Driver.Id == 0)
							expense.Driver = null;
						session.Save(expense);
					}

					transaction.Commit();
					session.Flush();
				}
			}

			return expense;
		}

		public TruckExpense GetTruckExpenseById(long expenseId)
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				return session.Get<TruckExpense>(expenseId);
			}
		}

		public bool DeleteTruckExpense(long expenseId)
		{
			bool result = false;
			using (ISession session = SessionFactory.OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					try
					{
						TruckExpense db_truckExpense = session.Get<TruckExpense>(expenseId);
						session.Delete(db_truckExpense);
						transaction.Commit();
						session.Flush();

						result = true;
					}
					catch
					{
						result = false;
					}

				}
			}

			return result;
		}

		public IList<DriverCash> GetDriverCashList(int driverId, DateTime? from = null, DateTime? to = null)
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				IQueryOver<DriverCash, DriverCash> queryOver = session.QueryOver<DriverCash>();
				queryOver.Inner.JoinQueryOver<Driver>(y => y.Driver).Where(z => z.Id == driverId);

				if (from.HasValue)
					queryOver.Where(x => x.Date >= from.Value);

				if (to.HasValue)
					queryOver.Where(x => x.Date <= to.Value);

				return queryOver.List();
			}
		}

		public DriverCash SaveDriverCash(DriverCash cash)
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					if (cash.Id > 0)
					{
						DriverCash db_cash = session.Get<DriverCash>(cash.Id);

						db_cash.Driver = session.Get<Driver>(cash.Driver.Id);
						db_cash.Amount = cash.Amount;
						db_cash.Date = cash.Date;
						db_cash.VoucherNumber = cash.VoucherNumber;

						session.Save(db_cash);
						cash = db_cash;
					}
					else
					{
						session.Save(cash);
					}

					transaction.Commit();
					session.Flush();
				}
			}

			return cash;
		}

		public DriverCash GetDriverCashById(long cashId)
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				return session.Get<DriverCash>(cashId);
			}
		}

		public bool DeleteDriverCash(long cashId)
		{
			bool result = false;
			using (ISession session = SessionFactory.OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					try
					{
						DriverCash db_driverCash = session.Get<DriverCash>(cashId);
						session.Delete(db_driverCash);
						transaction.Commit();
						session.Flush();

						result = true;
					}
					catch
					{
						result = false;
					}

				}
			}

			return result;
		}

		public IList<TruckExpense> GetTruckSheetExpenses(int truckId, int month, int year)
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				IQueryOver<TruckExpense, TruckExpense> query = session.QueryOver<TruckExpense>();
				query.Inner.JoinQueryOver<ExpensesSection>(y => y.Section);
				query.Where(x => x.Year == year).Where(x => x.Month == month).Where(x => x.Truck.Id == truckId);

				return query.List();
			}
		}

		public MonthAminPercentage GetMonthPercentageByMonthYear(int month, int year)
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				return session.QueryOver<MonthAminPercentage>().Where(x => x.Month == month).Where(x => x.Year == year).List().FirstOrDefault();
			}
		}

		public IList<DriverSalary> GetDriverSalaries(int driverId)
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				return session.QueryOver<DriverSalary>().Where(x => x.Driver.Id == driverId).List();
			}
		}

		public DriverSalary GetDriverSalaryById(long driverSalaryId)
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				return session.Get<DriverSalary>(driverSalaryId);
			}
		}

		public DriverSalary SaveDriverSalary(DriverSalary salary)
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					if (salary.Id > 0)
					{
						DriverSalary db_salary = session.Get<DriverSalary>(salary.Id);
						db_salary.Amount = salary.Amount;
						db_salary.Year = salary.Year;
						db_salary.Month = salary.Month;

						session.Save(db_salary);
						salary = db_salary;
					}
					else
					{
						session.Save(salary);
					}

					transaction.Commit();
					session.Flush();
				}
			}

			return salary;
		}

		public bool DeleteDriverSalary(long salaryId)
		{
			bool result = false;
			using (ISession session = SessionFactory.OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					try
					{
						DriverSalary db_salary = session.Get<DriverSalary>(salaryId);
						session.Delete(db_salary);
						transaction.Commit();
						session.Flush();

						result = true;
					}
					catch
					{
						result = false;
					}

				}
			}

			return result;
		}

		public DriverSalary GetDriverMonthSalary(int driverId, int month, int year)
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				return session.QueryOver<DriverSalary>().Where(x => x.Driver.Id == driverId && x.Month == month && x.Year == year).List().FirstOrDefault();
			}
		}

		public IList<TruckExpense> GetTruckExpensesByDriver(int driverId, int month, int year)
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				return session.QueryOver<TruckExpense>().Where(x => x.Driver.Id == driverId && x.Month == month && x.Year == year).List();
			}
		}

		public DriverMonthBalance GetDriverMonthBalance(int driverId, int month, int year)
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				return session.QueryOver<DriverMonthBalance>().Where(x => x.Driver.Id == driverId && x.Month == month && x.Year == year).List().FirstOrDefault();
			}
		}

		public bool UpdateDriverMonthBalanace(int driverId, int month, int year, decimal balanace)
		{
			bool result = false;
			using (ISession session = SessionFactory.OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					try
					{
						DriverMonthBalance monthBalanace = this.GetDriverMonthBalance(driverId, month, year);
						if (monthBalanace != null)
						{
							monthBalanace = session.Get<DriverMonthBalance>(monthBalanace.Id);
							monthBalanace.Amount = balanace;
						}
						else
						{
							monthBalanace = new DriverMonthBalance();
							monthBalanace.Amount = balanace;
							monthBalanace.Month = month;
							monthBalanace.Year = year;
							monthBalanace.Driver = new Driver { Id = driverId };
						}

						monthBalanace.LastModefied = DateTime.Now;
						session.Save(monthBalanace);

						transaction.Commit();
						session.Flush();

						result = true;
					}
					catch
					{
						result = false;
					}

				}
			}

			return result;
		}

		public IList<MaterialAdminPercentage> GetMaterialsAdminPercentageByMonth(int monthId)
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				return session.QueryOver<MaterialAdminPercentage>().Where(x => x.Month.Id == monthId).Cacheable().List();
			}
		}

		public MonthAminPercentage GetMonthAminPercentageById(int monthId)
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				return session.Get<MonthAminPercentage>(monthId);
			}
		}

		public void SaveMonthAdminPercentage(MonthAminPercentage month, IList<MaterialAdminPercentage> monthMaterialPercentages,
			IList<long> materialsToDelete, IList<long> customersToDelete)
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					//delete deleted items
					foreach (long customerId in customersToDelete)
					{
						CustomerAdminPercentage customer_db = session.Get<CustomerAdminPercentage>(customerId);
						session.Delete(customer_db);
					}
					foreach (long materialId in materialsToDelete)
					{
						MaterialAdminPercentage material_db = session.Get<MaterialAdminPercentage>(materialId);
						foreach (CustomerAdminPercentage customerPercentage in material_db.CustomersPercentage)
						{
							CustomerAdminPercentage customer_db = session.Get<CustomerAdminPercentage>(customerPercentage.Id);
							session.Delete(customer_db);
						}
						session.Flush();

						session.Delete(material_db);
					}
					session.Flush();

					//add new data
					MaterialAdminPercentage materialPercentage_db = null;
					CustomerAdminPercentage customerPercentage_db = null;

					if (monthMaterialPercentages != null)
					{ 
						foreach (MaterialAdminPercentage materialPercentage in monthMaterialPercentages)
						{
							if (materialPercentage.Id > 0)
							{ 
								materialPercentage_db = session.Get<MaterialAdminPercentage>(materialPercentage.Id);
								materialPercentage_db.Material = session.Get<Material>(materialPercentage.Material.Id);
								materialPercentage_db.Amount = materialPercentage.Amount;
								materialPercentage_db.IsFixedAmount = materialPercentage.IsFixedAmount;
								session.Save(materialPercentage_db);
							}
							else
							{
								materialPercentage.Material = session.Get<Material>(materialPercentage.Material.Id);
								session.Save(materialPercentage);

								materialPercentage_db = materialPercentage;
							}
						
							if (materialPercentage.CustomersPercentage != null) //stupid I know!
							{
								foreach (CustomerAdminPercentage customerPercentage in materialPercentage.CustomersPercentage)
								{
									if (customerPercentage.Id > 0)
									{
										customerPercentage_db = session.Get<CustomerAdminPercentage>(customerPercentage.Id);
										customerPercentage.Customer = session.Get<Customer>(customerPercentage.Customer.Id);
										customerPercentage_db.MaterialPercentage = materialPercentage_db;
										customerPercentage_db.Amount = customerPercentage.Amount;
										customerPercentage_db.IsFixedAmount = customerPercentage.IsFixedAmount;
									
										session.Save(customerPercentage_db);
									}
									else
									{
										customerPercentage.MaterialPercentage = materialPercentage_db;
										customerPercentage.Customer = session.Get<Customer>(customerPercentage.Customer.Id);
										session.Save(customerPercentage);
									}
								}
							}
						}
					}
					session.Flush();

					transaction.Commit();
				}
			}
		}

		public IList<MonthAminPercentage> GetAllMonthAdminPercentages()
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				return session.QueryOver<MonthAminPercentage>().List();
			}
		}

		public MonthAminPercentage SaveMonthAdminPercentage(MonthAminPercentage month, int? monthToCopyFrom)
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					if (month.Id > 0)
					{
						MonthAminPercentage db_month = session.Get<MonthAminPercentage>(month.Id);
						db_month.Amount = month.Amount;
						db_month.Year = month.Year;
						db_month.Month = month.Month;

						session.Save(db_month);
						month = db_month;
					}
					else
					{
						session.Save(month);

						//copy data from a previous month
						if (monthToCopyFrom.HasValue)
						{
							IList<MaterialAdminPercentage> materials = this.GetMaterialsAdminPercentageByMonth(monthToCopyFrom.Value);
							foreach (MaterialAdminPercentage material in materials)
							{
								MaterialAdminPercentage newMaterial = new MaterialAdminPercentage();
								newMaterial.Month = month;
								newMaterial.Material = session.Get<Material>(material.Material.Id);
								newMaterial.Amount = material.Amount;
								newMaterial.IsFixedAmount = material.IsFixedAmount;

								session.Save(newMaterial);

								//save customers
								foreach (CustomerAdminPercentage customer in material.CustomersPercentage)
								{
									CustomerAdminPercentage newCustomer = new CustomerAdminPercentage();
									newCustomer.MaterialPercentage = newMaterial;
									newCustomer.Customer = session.Get<Customer>(customer.Customer.Id);
									newCustomer.Amount = customer.Amount;
									newCustomer.IsFixedAmount = customer.IsFixedAmount;

									session.Save(newCustomer);
								}
							}
						}
					}

					transaction.Commit();
					session.Flush();
				}
			}

			return month;
		}

		public bool DeleteMonthAdminPercentage(int monthId)
		{
			bool result = false;
			using (ISession session = SessionFactory.OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					try
					{
						MonthAminPercentage db_month = session.Get<MonthAminPercentage>(monthId);
						session.Delete(db_month);
						transaction.Commit();
						session.Flush();

						result = true;
					}
					catch
					{
						result = false;
					}

				}
			}

			return result;
		}

		public IList<DateTime> GetDistinctMonths()
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				string sql = @"SELECT DISTINCT DATEPART(month, [TripDate]) AS _m, DATEPART(year, [TripDate]) AS _y FROM [dbo].[Trips] ORDER BY _y DESC, _m DESC;";
				var monthsSQLData = session.CreateSQLQuery(sql).List();
				IList<DateTime> monthsList = new List<DateTime>();
				foreach (object driverObj in monthsSQLData)
				{
					object[] properties = driverObj as object[];
					if(properties != null)
					{ 
						DateTime monthDate = new DateTime((int)properties[1], (int)properties[0], 1);
						monthsList.Add(monthDate);
					}
				}

				return monthsList;
			}
		}

		public IList<Trip> GetMonthTrips(int month, int year)
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				return session.QueryOver<Trip>().Where(p => p.Date.Month == month && p.Date.Year == year).List();
			}
		}

		public IList<TruckExpense> GetMonthTruckExpenses(int month, int year)
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				return session.QueryOver<TruckExpense>().Where(p => p.Month == month && p.Year == year).List();
			}
		}

		public IList<DriverSalary> GetMonthDriverSalaries(int month, int year, IEnumerable<int> driversIds = null)
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				if (driversIds != null)
					return session.QueryOver<DriverSalary>().Where(p => p.Month == month && p.Year == year).AndRestrictionOn( x => x.Driver.Id).IsIn(driversIds.ToArray()).List();
				else
					return session.QueryOver<DriverSalary>().Where(p => p.Month == month && p.Year == year).List();
			}
		}

		public IList<DriverCash> GetMonthDriverRecievedCash(int month, int year)
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				return session.QueryOver<DriverCash>().Where(p => p.Date.Month == month && p.Date.Year == year).List();
			}
		}
	}
}
