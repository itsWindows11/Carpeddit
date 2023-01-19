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
    public partial class SqliteRepository : IRepository
    {
        private SQLiteAsyncConnection _asyncDb;

        public async Task InitializeAsync()
        {
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync("Cache.db", CreationCollisionOption.OpenIfExists);

            _asyncDb ??= new SQLiteAsyncConnection(file.Path);

            await _asyncDb.EnableWriteAheadLoggingAsync();

            _ = await Task.WhenAll(
                    //_asyncDb.CreateTableAsync<CachedPost>(),
                    _asyncDb.CreateTableAsync<CachedUser>()
                );
        }

        public async Task<IEnumerable<T>> GetItemsAsync<T>() where T : DbObject, new()
            => await _asyncDb.Table<T>().ToListAsync();

        public Task InsertAsync<T>(T item) where T : DbObject, new()
            => _asyncDb.InsertAsync(item);

        public Task UpdateAsync<T>(T item) where T : DbObject, new()
            => _asyncDb.UpdateAsync(item);

        public Task UpsertAsync<T>(T item) where T : DbObject, new()
            => _asyncDb.InsertOrReplaceAsync(item);

        public Task ClearAsync<T>() where T : DbObject, new()
            => _asyncDb.DeleteAllAsync<T>();

        public Task DeleteAsync<T>(T item) where T : DbObject, new()
            => _asyncDb.DeleteAsync<T>(item);
    }

    // Synchronous implementation.
    public partial class SqliteRepository : ISyncRepository
    {
        private SQLiteConnection _db;

        public void Initialize()
        {
            var path = ApplicationData.Current.LocalFolder.Path;

            using var _ = File.Open(path, FileMode.OpenOrCreate);

            _db = new SQLiteConnection(path);
        }

        public IEnumerable<T> GetItems<T>() where T : DbObject, new()
            => _db.Table<T>().ToList();

        public void Insert<T>(T item) where T : DbObject, new()
            => _db.Insert(item);

        public void Update<T>(T item) where T : DbObject, new()
            => _db.Update(item);

        public void Upsert<T>(T item) where T : DbObject, new()
            => _db.InsertOrReplace(item);

        public void Clear<T>(T item) where T : DbObject, new()
            => _db.DeleteAll<T>();

        public void Delete<T>(T item) where T : DbObject, new()
            => _db.Delete(item);
    }
}
