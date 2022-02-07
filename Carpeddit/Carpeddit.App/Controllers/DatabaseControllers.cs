using Carpeddit.App.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Carpeddit.App.Controllers
{
    public class AccountDatabaseController
    {
        private static StorageFile _dbFile;

        public AccountDatabaseController()
        {
            _ = Init();
        }

        public static async Task<AccountDatabaseController> Init()
        {
            _dbFile = await ApplicationData.Current.LocalCacheFolder.CreateFileAsync($"Accounts.json", CreationCollisionOption.OpenIfExists);
            return new AccountDatabaseController();
        }

        public async Task<CustomAccountModel> GetAsync()
        {
            string text = await FileIO.ReadTextAsync(_dbFile);
            if (!string.IsNullOrWhiteSpace(text))
            {
                CustomAccountModel account = JsonConvert.DeserializeObject<CustomAccountModel>(await FileIO.ReadTextAsync(_dbFile));
                return account ?? new CustomAccountModel();
            }
            else
            {
                return new CustomAccountModel();
            }
        }

        public async Task InsertAsync(CustomAccountModel model)
        {
            string json = JsonConvert.SerializeObject(model, Formatting.Indented);
            await FileIO.WriteTextAsync(_dbFile, json);
        }

        public async Task UpdateAsync(CustomAccountModel model)
        {
            string json = JsonConvert.SerializeObject(model, Formatting.Indented);
            await FileIO.WriteTextAsync(_dbFile, json);
        }
    }
}
