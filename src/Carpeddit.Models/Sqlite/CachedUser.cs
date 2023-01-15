using Carpeddit.Models.Sqlite;
using SQLite;

namespace Carpeddit.Models
{
    /// <summary>
    /// Contains the most frequently used values of a
    /// Reddit user model, that can be locally cached.
    /// </summary>
    [Table("CachedUsers")]
    public sealed class CachedUser : DbObject
    {

    }
}
