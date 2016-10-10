using System;
using WDAdmin.Domain.Abstract;
using WDAdmin.Domain.Entities;

namespace WDAdmin.WebUI.Infrastructure
{
    /// <summary>
    /// Handler for Create/Update/Delete entity operations
    /// </summary>
    public sealed class EntityOpHandler
    {
        private static EntityOpHandler _instance;
        private IGenericRepository _repository;

        /// <summary>
        /// GetInstance method for singleton use
        /// </summary>
        public static EntityOpHandler GetInstance
        {
            get { return _instance ?? (_instance = new EntityOpHandler()); }
        }

        /// <summary>
        /// Initialize repository
        /// </summary>
        /// <param name="repository">IGenericRepository</param>
        public void SetRepository(IGenericRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Create new entity in repository
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="entity">T</param>
        /// <param name="errorLogTitle">string</param>
        /// <param name="otherInfo">string</param>
        /// <param name="logType">LogType</param>
        /// <returns>bool</returns>
        public bool CreateEntity<T>(T entity, string errorLogTitle, string otherInfo, LogType logType) where T : class
        {
            try
            {
                _repository.Create(entity);
            }
            catch (Exception ex)
            {
                Logger.Log(errorLogTitle, ex.Message, otherInfo, logType, LogEntryType.Error);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Update existing entity in repository
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="entity">T</param>
        /// <param name="errorLogTitle">string</param>
        /// <param name="otherInfo">string</param>
        /// <param name="logType">LogType</param>
        /// <returns>bool</returns>
        public bool UpdateEntity<T>(T entity, string errorLogTitle, string otherInfo, LogType logType) where T : class
        {
            try
            {
                _repository.Update(entity);
            }
            catch (Exception ex)
            {
                Logger.Log(errorLogTitle, ex.Message, otherInfo, logType, LogEntryType.Error);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Delete entity from repository
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="entity">T</param>
        /// <param name="errorLogTitle">string</param>
        /// <param name="otherInfo">string</param>
        /// <param name="logType">LogType</param>
        /// <returns>bool</returns>
        public bool DeleteEntity<T>(T entity, string errorLogTitle, string otherInfo, LogType logType) where T : class
        {
            try
            {
                _repository.Delete(entity);
            }
            catch (Exception ex)
            {
                Logger.Log(errorLogTitle, ex.Message, otherInfo, logType, LogEntryType.Error);
                return false;
            }

            return true;
        }
    }
}