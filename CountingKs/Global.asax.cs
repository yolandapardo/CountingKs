using Ninject.Web.Common.WebHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Ninject;
using Ninject.Web.Common;
using WebApiContrib.IoC.Ninject;
using CountingKs.Data;
using CountingKs.Services;

namespace CountingKs
{
  // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
  // visit http://go.microsoft.com/?LinkId=9394801

  public class MvcApplication : NinjectHttpApplication
  {
    protected  override void OnApplicationStarted()
    {
      base.OnApplicationStarted();
      AreaRegistration.RegisterAllAreas();

      GlobalConfiguration.Configure(WebApiConfig.Register);
      FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
      RouteConfig.RegisterRoutes(RouteTable.Routes);
      BundleConfig.RegisterBundles(BundleTable.Bundles);
      AuthConfig.RegisterAuth();
    }
        protected override IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
            GlobalConfiguration.Configuration.DependencyResolver = new NinjectResolver(kernel);
         //   GlobalConfiguration.Configuration.Services.Add(typeof(IFilterProvider), new NinjectWebApiFilterProvider(kernel));
            RegisterServices(kernel);
            return kernel;
        }

        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<ICountingKsRepository>().To<CountingKsRepository>();
            kernel.Bind<CountingKsContext>().To<CountingKsContext > ();
            kernel.Bind<ICountingKsIdentityService>().To<CountingKsIdentityService>();

        }
    }
}