using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;

namespace WDAdmin.Domain.Abstract
{
    //Interface implementation for GenericRepository
    public interface IVRTGenericRepository
    {
        IQueryable<TEntity> Get<TEntity>() where TEntity : class;
        void Create<TEntity>(TEntity entity) where TEntity : class;
        void Update<TEntity>(TEntity entity) where TEntity : class;
        void Delete<TEntity>(TEntity entity) where TEntity : class;
    }
}
