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
        public int Id { get; set; }
        
        [Column]
        public string Name { get; set; }

        [Column]
        public string Description { get; set; }

        [Column]
        public string Url { get; set; }

        [Column]
        public int Count { get; set; }

        [Column]
        public int UserGroupId { get; set; }

        [Column]
        public int UserId { get; set; }

        [Column]
        public DateTime ReleaseDate { get; set; }

        [Column(CanBeNull = true)]
        public string Password { get; set; }

        [Column]
        public int VideoCategoryId { get; set; }
    }
}