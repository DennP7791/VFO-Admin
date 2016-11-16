using System;
using System.Data.Linq.Mapping;

namespace WDAdmin.Domain.Entities
{
    /// <summary>
    /// Entity class for SQL table - Video
    /// </summary>
    [Table]
    public class Video
    {
        [Column(IsPrimaryKey = true, CanBeNull = false, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public Guid Id { get; set; }
        
        [Column]
        public string Name { get; set; }

        [Column]
        public string Description { get; set; }

        [Column]
        public string Path { get; set; }

        [Column]
        public int Count { get; set; }

        [Column]
        public int UserGroupId { get; set; }

        [Column]
        public int UserId { get; set; }

        [Column(CanBeNull = true)]
        public DateTime? ReleaseDate { get; set; }

        [Column]
        public int VideoCategoryId { get; set; }
    }
}