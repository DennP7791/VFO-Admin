using System;
using System.Linq;
using WDAdmin.Domain.Entities;

namespace WDAdmin.WebUI
{
    /// <summary>
    /// Class Extensions.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// To the javascript timestamp.
        /// </summary>
        /// <param name="datetime">The datetime.</param>
        /// <returns>System.Int64.</returns>
        public static long ToJavascriptTimestamp(this DateTime datetime)
        {
            var span = new TimeSpan(DateTime.Parse("1/1/1970").Ticks);
            var time = datetime.Subtract(span);
            return (time.Ticks / 10000);
        }


        /// <summary>
        /// Joins the child groups.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="collection">The collection.</param>
        /// <returns>IQueryable&lt;GroupRelationship&gt;.</returns>
        public static IQueryable<GroupRelationship> JoinChildGroups(this IQueryable<UserGroup> selection, IQueryable<UserGroup> collection)
        {
            return selection.Select(x => new GroupRelationship
            {
                Parent = x,
                Children = collection
                    .Where(y => y.UserGroupParentId == x.Id)
                    .JoinChildGroups(collection),
            });
        }

        /// <summary>
        /// Selects the child groups.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="collection">The collection.</param>
        /// <returns>IQueryable&lt;UserGroup&gt;.</returns>
        public static IQueryable<UserGroup> SelectChildGroups(this IQueryable<UserGroup> selection, IQueryable<UserGroup> collection)
        {
            return selection
                .SelectMany(x => collection.Where(y => y.UserGroupParentId == x.Id))
                .SelectChildGroups(collection);
        }
    }

    public class GroupRelationship
    {
        public UserGroup Parent { get; set; }
        public IQueryable<GroupRelationship> Children { get; set; }
    }
}