using System.Data.Linq.Mapping;

namespace WDAdmin.Domain.Entities
{
    /// <summary>
    /// Entity class for SQL table - ExerciseDetails
    /// </summary>
    [Table]
    public class ExerciseDetails
    {
        [Column(IsDbGenerated = true, IsPrimaryKey = true, AutoSync = AutoSync.OnInsert)]
        public int Id { get; set; }
        [Column]
        public string Name { get; set; }
        [Column]
        public int SceneFunction { get; set; }
        [Column]
        public int CategoryId { get; set; }
        [Column]
        public int? OrderNr { get; set; }

        //Helper variables
        public bool IsChosen { get; set; }
    }
}