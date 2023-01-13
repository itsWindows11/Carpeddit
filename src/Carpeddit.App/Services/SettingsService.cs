using Windows.Foundation.Collections;
using Windows.Storage;

namespace Carpeddit.App.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly IPropertySet _settings = ApplicationData.Current.LocalSettings.Values;

        public T GetValue<T>(string key)
        {
            if (_settings.TryGetValue(key, out object value))
                return (T)value;

            return default;
        }

        public void SetValue<T>(string key, T value)
        {
            if (!_settings.ContainsKey(key))
            {
                _settings.Add(key, value);
                return;
            }

            _settings[key] = value;
        }
    }
}
