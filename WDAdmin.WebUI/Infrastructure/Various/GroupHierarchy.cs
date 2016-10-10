using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WDAdmin.Domain.Abstract;
using WDAdmin.Domain.Entities;
using WDAdmin.WebUI.Models;

namespace WDAdmin.WebUI.Infrastructure.Various
{
    /// <summary>
    /// Class GroupHierarchyBuilder.
    /// </summary>
    public class GroupHierarchyBuilder
    {
        /// <summary>
        /// The _repository
        /// </summary>
        private IGenericRepository _repository;
        /// <summary>
        /// The _count users
        /// </summary>
        private bool _countUsers;
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupHierarchyBuilder"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="countUsers">if set to <c>true</c> [count users].</param>
        public GroupHierarchyBuilder(IGenericRepository repository, bool countUsers)
        {
            _repository = repository;
            _countUsers = countUsers;
        }
        /// <summary>
        /// Gets the specified GTG rights.
        /// </summary>
        /// <param name="gtgRights">The GTG rights.</param>
        /// <returns>GroupNode.</returns>
        public GroupNode Get(List<GroupToGroupRight> gtgRights)
        {
            var ids = gtgRights.Select(x => x.Id).ToArray();
            var groups = from ug in _repository.Get<UserGroup>()
                         where ids.Contains(ug.Id)
                         select new GroupNode {
                             Id = ug.Id,
                             CountryId = ug.CountryId,
                             GroupName = ug.GroupName,
                             UserGroupParentId = ug.UserGroupParentId,
                         };
            GroupNode hierachy = groups.First();
            hierachy.ChildGroups = GetChildGroups(groups.OrderBy(x => x.GroupName), hierachy.Id);
            return hierachy;
        }

        /// <summary>
        /// Gets the child groups.
        /// </summary>
        /// <param name="groups">The groups.</param>
        /// <param name="parentId">The parent identifier.</param>
        /// <returns>List&lt;GroupNode&gt;.</returns>
        private List<GroupNode> GetChildGroups(IQueryable<GroupNode> groups, int parentId)
        {
            var childGroups = (from cg in groups
                              where cg.UserGroupParentId == parentId
                              select cg).ToList();
            if (childGroups.Any())
            {
                foreach (var cg in childGroups)
                {
                    cg.UsersCount = _repository.Get<User>().Where(x => x.UserGroupId == cg.Id).Count();
                    cg.ChildGroups = GetChildGroups(groups, cg.Id);
                }
            }
            return childGroups;
        }
    }

    public class GroupNode
    {
        public int Id { get; set; }
        public string GroupName { get; set; }
        public int CountryId { get; set; }
        public int? UserGroupParentId { get; set; }
        public List<GroupNode> ChildGroups { get; set; }
        public int UsersCount { get; set; }
    }
}