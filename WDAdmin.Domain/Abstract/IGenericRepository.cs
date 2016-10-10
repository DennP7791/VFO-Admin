using System.Linq;

namespace WDAdmin.Domain.Abstract
{
    /// <summary>
    /// Interface definition for GenericRepository
    /// </summary>
    public interface IGenericRepository
    {
        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <returns>IQueryable&lt;TEntity&gt;.</returns>
        IQueryable<TEntity> Get<TEntity>() where TEntity : class;
        /// <summary>
        /// Creates the specified entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <param name="entity">The entity.</param>
        void Create<TEntity>(TEntity entity) where TEntity : class;
        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <param name="entity">The entity.</param>
        void Update<TEntity>(TEntity entity) where TEntity : class;
        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <param name="entity">The entity.</param>
        void Delete<TEntity>(TEntity entity) where TEntity : class;
    }
}