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
				{ 
					queryOver.Inner.JoinQueryOver<Site>(y => y.Site).Where(z => z.Customer.Id == parameters.CustomerId);
				}
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
			using (ISession session = SessionFactory.OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					if (driver.Id > 0)
					{
						Driver db_driver = session.Get<Driver>(driver.Id);



						db_driver.Name = driver.Name;
						db_driver.IsActive = driver.IsActive;
						db_driver.IsOutsourced = driver.IsOutsourced;
						

						session.Save(db_driver);
						driver = db_driver;
					}
					else
					{
						session.Save(driver);
					}

					transaction.Commit();
					session.Flush();
				}
			}

			return driver;
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
			using (ISession session = SessionFactory.OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					if (truck.Id > 0)
					{
						Truck db_truck = session.Get<Truck>(truck.Id);

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
					}

					transaction.Commit();
					session.Flush();
				}
			}

			return truck;
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
	}
}
