using System;
using System.Data.Linq.Mapping;

namespace WDAdmin.Domain.Entities
{
    /// <summary>
    /// Entity class for SQL table - Exercise
    /// </summary>
    [Table]
    public class Exercise
    {
        [Column(IsDbGenerated = true, IsPrimaryKey = true, AutoSync = AutoSync.OnInsert)]
        public int Id { get; set; }
        [Column]
        public int DetailsId { get; set; }
        [Column]
        public int CategoryId { get; set; }
        [Column]
        public double Score { get; set; }
        [Column]
        public DateTime Timestamp { get; set; }
        [Column]
        public bool Attempted { get; set; }

        //Helper variables
        public string ExerciseName { get; set; }
        public bool IsChosen { get; set; }
    }
}