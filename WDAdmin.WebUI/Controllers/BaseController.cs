using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using WDAdmin.Domain.Abstract;
using WDAdmin.Domain.Entities;
using WDAdmin.WebUI.Infrastructure;
using WDAdmin.WebUI.Models;

namespace WDAdmin.WebUI.Controllers
{
	/// <summary>
	/// Base controller with shared functionalities
	/// </summary>
	public class BaseController : Controller
	{
		private readonly IGenericRepository _repository;
        protected internal readonly string DefaultCulture = ConfigurationManager.AppSettings["DefaultCulture"];

		public BaseController(IGenericRepository repository)
		{
			_repository = repository;
		}

		/// <summary>
		/// Create new entity in repository
		/// </summary>
		/// <typeparam name="T">Entity type</typeparam>
		/// <param name="entity">Entity object of type</param>
		/// <param name="errorLogTitle">Title for error log</param>
		/// <param name="otherInfo">Extra log info</param>
		/// <param name="logType">Log type</param>
		/// <returns>Result of the operation</returns>
		protected internal bool CreateEntity<T>(T entity, string errorLogTitle, string otherInfo, LogType logType) where T : class
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
		/// <typeparam name="T">Entity type</typeparam>
		/// <param name="entity">Entity object of type</param>
		/// <param name="errorLogTitle">Title for error log</param>
		/// <param name="otherInfo">Extra log info</param>
		/// <param name="logType">Log type</param>
		/// <returns>Result of the operation</returns>
		protected internal bool UpdateEntity<T>(T entity, string errorLogTitle, string otherInfo, LogType logType) where T : class
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
		/// <typeparam name="T">Entity type</typeparam>
		/// <param name="entity">Entity object of type</param>
		/// <param name="errorLogTitle">Title for error log</param>
		/// <param name="otherInfo">Extra log info</param>
		/// <param name="logType">Log type</param>
		/// <returns>Result of the operation</returns>
		protected internal bool DeleteEntity<T>(T entity, string errorLogTitle, string otherInfo, LogType logType) where T : class
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

		/// <summary>
		/// Get (recursively) parent group responsible users
		/// </summary>
		/// <param name="responsibleUsers">List of responsible users</param>
		/// <param name="groupId">Group ID</param>
		/// <param name="responsibleId">ID of the responsible user</param>
		protected internal void GetGroupsResponsibleUsers(List<GroupsResponsible> responsibleUsers, int groupId, int responsibleId)
		{
			//Get allowed user groups from MasterModel
			var gtgRights = ((MasterUserRightsModel)Session["Rights"]).GroupToGroupViewRights;

			if (gtgRights.Any(x => x.Id == groupId))
			{
				//Find administrative users in chosen group
				var users = (from ug in _repository.Get<UserGroup>()
							 where ug.Id == groupId
							 join us in _repository.Get<User>() on ug.Id equals us.UserGroupId
							 where us.SalaryNumber == null && us.IsDeleted == false
							 select new { User = us, Group = ug }).ToList();

				foreach (var use in users)
				{
					var isResponsible = responsibleId == use.User.Id ? true : false;

					responsibleUsers.Add(new GroupsResponsible
					{
						UserId = use.User.Id,
						IsResponsible = isResponsible,
						Username = use.User.Firstname + " " + use.User.Lastname + "  (" + use.Group.GroupName + ")"
					});
				}

				//Find group's parent group - if exists and allowed, find users in that group
				var parentGroupId = (from pg in _repository.Get<UserGroup>() where pg.Id == groupId select pg.UserGroupParentId).FirstOrDefault();

				if (parentGroupId != null && gtgRights.Any(x => x.Id == parentGroupId))
				{
					GetGroupsResponsibleUsers(responsibleUsers, (int)parentGroupId, responsibleId);
				}
			}
		}
	}
}