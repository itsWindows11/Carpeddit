using Carpeddit.Models.Sqlite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Carpeddit.Repository
{
    /// <summary>
    /// Represents a repository which can have
    /// data stored in, anywhere.
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// Initializes the database.
        /// </summary>
        /// <returns>A <see cref="Task"/> which represents the operation.</returns>
        Task<IRepository> InitializeAsync();

        /// <summary>
        /// Gets a list of items from the database.
        /// </summary>
        /// <typeparam name="T">The type of the item to get.</typeparam>
        /// <returns>A list of items.</returns>
        IAsyncEnumerable<T> GetItemsAsync<T>() where T : DbObject, new();

        /// <summary>
        /// Inserts an item into the database.
        /// </summary>
        /// <typeparam name="T">The type of item to insert.</typeparam>
        /// <param name="item">The item to insert.</param>
        /// <returns>A <see cref="Task"/> which represents the operation.</returns>
        Task InsertAsync<T>(T item) where T : DbObject, new();

        /// <summary>
        /// Updates an item in the database.
        /// </summary>
        /// <typeparam name="T">The type of item to update.</typeparam>
        /// <param name="item">The item to update.</param>
        /// <returns>A <see cref="Task"/> which represents the operation.</returns>
        Task UpdateAsync<T>(T item) where T : DbObject, new();

        /// <summary>
        /// Inserts or updates an item into the database.
        /// </summary>
        /// <typeparam name="T">The type of item to upsert.</typeparam>
        /// <param name="item">The item to upsert.</param>
        /// <returns>A <see cref="Task"/> which represents the operation.</returns>
        Task UpsertAsync<T>(T item) where T : DbObject, new();
    }

    public interface ISyncRepository
    {
        /// <summary>
        /// Initializes the database.
        /// </summary>
        /// <returns>A <see cref="Task"/> which represents the operation.</returns>
        ISyncRepository Initialize();

        /// <summary>
        /// Gets a list of items from the database.
        /// </summary>
        /// <typeparam name="T">The type of the item to get.</typeparam>
        /// <returns>A list of items.</returns>
        IEnumerable<T> GetItems<T>() where T : DbObject, new();

        /// <summary>
        /// Inserts an item into the database.
        /// </summary>
        /// <typeparam name="T">The type of item to insert.</typeparam>
        /// <param name="item">The item to insert.</param>
        void Insert<T>(T item) where T : DbObject, new();

        /// <summary>
        /// Updates an item in the database.
        /// </summary>
        /// <typeparam name="T">The type of item to update.</typeparam>
        /// <param name="item">The item to update.</param>
        void Update<T>(T item) where T : DbObject, new();

        /// <summary>
        /// Inserts or updates an item into the database.
        /// </summary>
        /// <typeparam name="T">The type of item to upsert.</typeparam>
        /// <param name="item">The item to upsert.</param>
        void Upsert<T>(T item) where T : DbObject, new();
    }
}
