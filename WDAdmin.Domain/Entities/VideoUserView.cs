using System;
using System.Data.Linq.Mapping;

namespace WDAdmin.Domain.Entities
{
    /// <summary>
    /// Entity class for SQL table - VideoUserView
    /// </summary>
    [Table]
    public class VideoUserView
    {
        [Column(IsPrimaryKey = true, CanBeNull = false, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public int Id { get; set; }

        [Column]
        public Guid VideoId { get; set; }

        [Column]
        public int UserId { get; set; }

        [Column]
        public DateTime ViewDate { get; set; }
    }
}