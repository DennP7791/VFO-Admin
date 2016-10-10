using System.Data.Linq;
using WDAdmin.Domain.Abstract;

namespace WDAdmin.Domain.Concrete
{
    /// <summary>
    /// Implementation of DBContextProvider
    /// </summary>
    public class DbDataContextProvider : IDataContextProvider
    {
        /// <summary>
        /// The _data context
        /// </summary>
        private readonly DataContext _dataContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbDataContextProvider"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public DbDataContextProvider(string connectionString)
        {
            _dataContext = new DataContext(connectionString);
        }

        #region IDataContextProvider Members

        /// <summary>
        /// Gets the dc.
        /// </summary>
        /// <value>The dc.</value>
        public DataContext dc
        {
            get { return _dataContext; }
        }

        #endregion
    }
}