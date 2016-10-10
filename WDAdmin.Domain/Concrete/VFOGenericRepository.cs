using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WDAdmin.Domain.Abstract;
using System.Data.Linq;

namespace WDAdmin.Domain.Concrete
{
    //Implementation of GenericRepository for LINQ to SQL
    public class VFOGenericRepository : IVFOGenericRepository
    {
        private DataContext dataContext;

        public VFOGenericRepository(IVFODataContextProvider dataContextProvider)
        {
            dataContext = dataContextProvider.dc;
        }

        public IQueryable<TEntity> Get<TEntity>() where TEntity : class
        {
            return dataContext.GetTable<TEntity>().AsQueryable();
        }

        public void Create<TEntity>(TEntity entity) where TEntity : class
        {
            dataContext.GetTable<TEntity>().InsertOnSubmit(entity);
            dataContext.SubmitChanges();
        }

        public void Update<TEntity>(TEntity entity) where TEntity : class
        {
            var table = dataContext.GetTable<TEntity>();
            try
            {
                table.Attach(entity);
            }
            catch (InvalidOperationException e)
            { }

            dataContext.Refresh(RefreshMode.KeepCurrentValues, entity);
            dataContext.SubmitChanges();
        }

        public void Delete<TEntity>(TEntity entity) where TEntity : class
        {
            dataContext.GetTable<TEntity>().DeleteOnSubmit(entity);
            dataContext.SubmitChanges();
        }
    }
}
