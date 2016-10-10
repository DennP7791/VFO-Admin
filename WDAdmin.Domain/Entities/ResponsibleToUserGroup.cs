using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;

namespace WDAdmin.Domain.Entities
{
	/// <summary>
	/// Entity class for SQL table - ResponsibleToUserGroup
	/// </summary>
	[Table]
	public class ResponsibleToUserGroup
	{
		[Column(IsPrimaryKey = true, CanBeNull = false, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
		public int Id { get; set; }
		[Column]
		public int UserGroupId { get; set; }
		[Column]
		public int ResponsibleUserId { get; set; }
	}
}