using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;

namespace WDAdmin.Domain.Entities
{
    /// <summary>
    /// Entity class for SQL table - VideoCategory
    /// </summary>
    [Table]
    public class VideoCategory
    {
        [Column(IsPrimaryKey = true, CanBeNull = false, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public int Id { get; set; }

        [Column]
        public string Name { get; set; }

        /// <summary>
        /// Helper enum
        /// </summary>
        public enum CategoryType
        {
            Vejledning = 1,
            Forflytning = 2,
            IndividuelForflytning = 3
        }
    }
}
