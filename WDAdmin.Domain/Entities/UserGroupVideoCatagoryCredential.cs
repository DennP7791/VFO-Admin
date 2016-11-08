using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;

namespace WDAdmin.Domain.Entities
{
    /// <summary>
    /// Entity class for SQL table - UserGroupVideoCategoryCredential
    /// </summary>
    [Table]
    public class UserGroupVideoCatagoryCredential
    {
        [Column(IsPrimaryKey = true, CanBeNull = false, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public int Id { get; set; }
        
        [Column]
        public int VideoCatagoryId { get; set; }

        [Column]
        public int UserGroupId { get; set; }

        [Column]
        public string Password { get; set; }

        [Column]
        public string Salt { get; set; }
    }
}
