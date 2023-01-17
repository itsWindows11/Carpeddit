using Carpeddit.Models.Sqlite;
using SQLite;
using System;

namespace Carpeddit.Models
{
    /// <summary>
    /// Contains the most frequently used values of a
    /// Reddit user model, that can be locally cached.
    /// </summary>
    [Table("CachedUsers")]
    public sealed class CachedUser : DbObject
    {
        public string Name { get; set; }

        public string IconUrl { get; set; }

        public string BannerUrl { get; set; }

        public DateTime Created { get; set; }
    }
}
