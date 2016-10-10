using System.Data.Linq.Mapping;

namespace WDAdmin.Domain.Entities
{
    /// <summary>
    /// Entity class for SQL table - Country
    /// </summary>
    [Table]
    public class Country
    {
        [Column(IsPrimaryKey = true, CanBeNull = false, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public int Id { get; set; }
        [Column]
        public string Name { get; set; }
        [Column]
        public string CultureCode { get; set; }
        [Column]
        public string Language { get; set; }
    }
}