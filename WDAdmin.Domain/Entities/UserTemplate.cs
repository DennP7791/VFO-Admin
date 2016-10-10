using System;
using System.Data.Linq.Mapping;

namespace WDAdmin.Domain.Entities
{
    /// <summary>
    /// Entity class for SQL table - UserTemplate
    /// </summary>
    [Table]
    public class UserTemplate
    {
        [Column(IsPrimaryKey = true, CanBeNull = false, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public int Id { get; set; }
        [Column]
        public string TemplateName { get; set; }
        [Column]
        public DateTime Created { get; set; }
        [Column]
        public int TemplateLevel { get; set; }
        [Column]
        public int? ParentTemplateId { get; set; }
        [Column]
        public bool IsActive { get; set; }
    }
}