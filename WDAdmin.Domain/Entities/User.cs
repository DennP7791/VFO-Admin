using System.Data.Linq.Mapping;

namespace WDAdmin.Domain.Entities
{
    /// <summary>
    /// Entity class for SQL table - User
    /// </summary>
    [Table]
    public class User
    {
        [Column(IsPrimaryKey = true, CanBeNull = false, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public int Id { get; set; }
        [Column]
        public int? SalaryNumber { get; set; }
        [Column]
        public string Firstname { get; set; }
        [Column]
        public string Lastname { get; set; }
        [Column]
        public int? Phone { get; set; }
        [Column]
        public string Email { get; set; }
        [Column]
        public string Password { get; set; }
        [Column]
        public string Salt { get; set; }
        [Column]
        public int UserGroupId { get; set; }
        [Column]
        public int? CountryId { get; set; }
        [Column]
        public int UserTemplateId { get; set; }
        [Column]
        public bool IsDeleted { get; set; }
        [Column]
        public string Username { get; set; }
    }
}