using Carpeddit.Models;
using Carpeddit.Models.Sqlite;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace Carpeddit.Repository
{
    public partial class SqliteRepository : IRepository, ISyncRepository
    {
        private SQLiteAsyncConnection _asyncDb;

        public async Task<IRepository> InitializeAsync()
        {
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync("Lists.db", CreationCollisionOption.OpenIfExists);

            _asyncDb ??= new SQLiteAsyncConnection(file.Path);

            await _asyncDb.EnableWriteAheadLoggingAsync();

            _ = await Task.WhenAll(
                    _asyncDb.CreateTableAsync<CachedPost>(),
                    _asyncDb.CreateTableAsync<CachedUser>()
                );

            return new SqliteRepository();
        }

        public async IAsyncEnumerable<T> GetItemsAsync<T>() where T : DbObject, new()
        {
            foreach (var item in await _asyncDb.Table<T>().ToListAsync())
                yield return item;
        }

        public Task InsertAsync<T>(T item) where T : DbObject, new()
            => _asyncDb.InsertAsync(item);

        public Task UpdateAsync<T>(T item) where T : DbObject, new()
            => _asyncDb.UpdateAsync(item);

        public Task UpsertAsync<T>(T item) where T : DbObject, new()
            => _asyncDb.InsertOrReplaceAsync(item);
    }

    // Synchronous implementation.
    public partial class SqliteRepository : ISyncRepository
    {
        private SQLiteConnection _db;

        public ISyncRepository Initialize()
        {
            var path = ApplicationData.Current.LocalFolder.Path;

            using var _ = File.Create(path);

            _db = new SQLiteConnection(path);

            return new SqliteRepository();
        }

        public IEnumerable<T> GetItems<T>() where T : DbObject, new()
            => _db.Table<T>().ToList();

        public void Insert<T>(T item) where T : DbObject, new()
            => _db.Insert(item);

        public void Update<T>(T item) where T : DbObject, new()
            => _db.Update(item);

        public void Upsert<T>(T item) where T : DbObject, new()
            => _db.InsertOrReplace(item);
    }
}
