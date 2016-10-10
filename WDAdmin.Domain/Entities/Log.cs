using System;
using System.Data.Linq.Mapping;

namespace WDAdmin.Domain.Entities
{
    /// <summary>
    /// Entity class for SQL table - Log
    /// </summary>
    [Table]
    public class Log
    {
        [Column(IsPrimaryKey = true, CanBeNull = false, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public long Id { get; set; }
        [Column]
        public DateTime Timestamp { get; set; }
        [Column]
        public int? UserId { get; set; }
        [Column]
        public string Message { get; set; }
        [Column]
        public LogType Type { get; set; }
        [Column]
        public string Title { get; set; }
        [Column]
        public string OtherInfo { get; set; }

        //Helper variables
        public string Information { get; set; }
        public string Time { get; set; }
        public string TypeDescription { get; set; }
    }
}