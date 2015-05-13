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
						db_trip.AddedOn = trip.LastModefied = DateTime.Now;

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
						trip.AddedOn = trip.LastModefied = DateTime.Now;

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
	}
}
