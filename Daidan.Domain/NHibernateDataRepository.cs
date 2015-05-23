using Daidan.Entities;
using NHibernate;
using NHibernate.Criterion;
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
						db_trip.LastModefied = DateTime.Now;

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
	}
}
