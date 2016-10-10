using Ninject.Modules;
using System.Configuration;
using WDAdmin.Domain.Abstract;
using WDAdmin.Domain.Concrete;

namespace WDAdmin.WebUI.Infrastructure
{
    /// <summary>
    /// Ninject binding service
    /// </summary>
    public class NinjectService : NinjectModule
    {
        /// <summary>
        /// Loads this instance.
        /// </summary>
        public override void Load()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["VFO"].ConnectionString;
            
            Bind<IDataContextProvider>().To<DbDataContextProvider>().WithConstructorArgument("connectionString",connectionString);
            Bind<IGenericRepository>().To<SqlGenericRepository>();
        }
    }
}