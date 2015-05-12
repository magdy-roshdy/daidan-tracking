using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NHibernate;
using Ninject;
using Daidan.Entities;
using Daidan.Domain;

namespace Daidan.Web.Infrastructure
{
	public class NinjectControllerFactory : DefaultControllerFactory
	{
		private IKernel kernel;
		public NinjectControllerFactory()
		{
			kernel = new StandardKernel();
			AddBindings();
		}

		protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType)
		{
			return controllerType == null
			? null
			: (IController)kernel.Get(controllerType);
		}

		private void AddBindings()
		{
			var configuration = new NHibernate.Cfg.Configuration();
			configuration.Configure();
			ISessionFactory sessionFactory = configuration.BuildSessionFactory();

			kernel.Bind<IDataRepository>()
				.To<NHibernateDataRepository>()
				.WithPropertyValue("SessionFactory", sessionFactory);
		}
	}
}