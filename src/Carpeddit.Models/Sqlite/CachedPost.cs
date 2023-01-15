using Carpeddit.Models.Sqlite;
using SQLite;

namespace Carpeddit.Models
{
    /// <summary>
    /// Contains the most frequently used values of a
    /// Reddit post model, that can be locally cached.
    /// </summary>
    [Table("CachedPosts")]
    public sealed class CachedPost : DbObject
    {

    }
}
