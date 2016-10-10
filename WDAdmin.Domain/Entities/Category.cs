using System;
using System.Data.Linq.Mapping;

namespace WDAdmin.Domain.Entities
{
    /// <summary>
    /// Entity class for SQL table - Category
    /// </summary>
    [Table]
    public class Category
    {
        [Column(IsDbGenerated = true, IsPrimaryKey = true, AutoSync = AutoSync.OnInsert)]
        public int Id { get; set; }
        [Column]
        public int DetailsId { get; set; }
        [Column]
        public double Score { get; set; }
        [Column]
        public int UserId { get; set; }
        [Column]
        public DateTime Timestamp { get; set; }
        
        //Helper variables
        public string CategoryName { get; set; }
    }
}