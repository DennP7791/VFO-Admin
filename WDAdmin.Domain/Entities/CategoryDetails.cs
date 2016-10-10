using System.Collections.Generic;
using System.Data.Linq.Mapping;

namespace WDAdmin.Domain.Entities
{
    /// <summary>
    /// Entity class for SQL table - CategoryDetails
    /// </summary>
    [Table]
    public class CategoryDetails
    {
        [Column(IsDbGenerated = true, IsPrimaryKey = true, AutoSync = AutoSync.OnInsert)]
        public int Id { get; set; }
        [Column]
        public string Name { get; set; }

        //Helper variables
        public List<ExerciseDetails> Exercises { get; set; }
        public bool IsChosen { get; set; }
    }
}