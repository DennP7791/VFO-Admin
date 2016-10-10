using System.Data.Linq.Mapping;

namespace WDAdmin.Domain.Entities
{
    /// <summary>
    /// Entity class for SQL table - GroupToExerciseRight
    /// </summary>
    [Table]
    public class GroupToExerciseRight
    {
        [Column(IsDbGenerated = true, IsPrimaryKey = true, AutoSync = AutoSync.OnInsert)]
        public int Id { get; set; }
        [Column]
        public int GroupId { get; set; }
        [Column]
        public int ExerciseId { get; set; }
        [Column]
        public bool IsChosen { get; set; }
    }
}
