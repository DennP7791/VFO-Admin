using System.Data.Linq.Mapping;

namespace WDAdmin.Domain.Entities
{
    /// <summary>
    /// Entity class for SQL table - UserGroup
    /// </summary>
    [Table]
    public class UserGroup
    {
        [Column(IsPrimaryKey = true, CanBeNull = false, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public int Id { get; set; }
        [Column]
        public string GroupName { get; set; }
        [Column]
        public int CountryId { get; set; }
        [Column]
        public int? UserGroupParentId { get; set; }
        [Column]
        public int? CustomerId { get; set; }
        [Column]
        public int? PackageId { get; set; }
        [Column]
        public string Path { get; set; }
    }
}