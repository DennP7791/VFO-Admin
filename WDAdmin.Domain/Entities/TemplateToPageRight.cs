using System.Data.Linq.Mapping;

namespace WDAdmin.Domain.Entities
{
    /// <summary>
    /// Entity class for SQL table - TemplateToPageRight
    /// </summary>
    [Table]
    public class TemplateToPageRight
    {
        [Column(IsPrimaryKey = true, CanBeNull = false, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public int Id { get; set; }
        [Column]
        public int UserTemplateId { get; set; }
        [Column]
        public int PageId { get; set; }
        [Column]
        public bool IsAllowed { get; set; }
    }
}