using System;
using System.Data.Linq;
using System.Linq;
using WDAdmin.Domain.Abstract;

namespace WDAdmin.Domain.Concrete
{
    /// <summary>
    /// Implementation of GenericRepository for LINQ to SQL
    /// </summary>
    public class SqlGenericRepository : IGenericRepository
    {
        /// <summary>
        /// The _data context
        /// </summary>
        private readonly DataContext _dataContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlGenericRepository"/> class.
        /// </summary>
        /// <param name="dataContextProvider">The data context provider.</param>
        public SqlGenericRepository(IDataContextProvider dataContextProvider)
        {
            _dataContext = dataContextProvider.dc;
        }

        #region IGenericRepository Members

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <returns>IQueryable&lt;TEntity&gt;.</returns>
        public IQueryable<TEntity> Get<TEntity>() where TEntity : class
        {
            return _dataContext.GetTable<TEntity>().AsQueryable();
        }

        /// <summary>
        /// Creates the specified entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public void Create<TEntity>(TEntity entity) where TEntity : class
        {
            _dataContext.GetTable<TEntity>().InsertOnSubmit(entity);
            _dataContext.SubmitChanges();
        }

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public void Update<TEntity>(TEntity entity) where TEntity : class
        {
            var table = _dataContext.GetTable<TEntity>();
            
            //Check if entity already attached - returns null if not attached
            var origstate = table.GetOriginalEntityState(entity);
            if(origstate == null)
            {
                table.Attach(entity);
            }

            _dataContext.Refresh(RefreshMode.KeepCurrentValues, entity);
            _dataContext.SubmitChanges();
        }

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public void Delete<TEntity>(TEntity entity) where TEntity : class
        {
            _dataContext.GetTable<TEntity>().DeleteOnSubmit(entity);
            _dataContext.SubmitChanges();
        }

        #endregion
    }
}