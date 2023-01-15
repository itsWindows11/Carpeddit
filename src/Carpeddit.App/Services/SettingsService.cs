using System.Runtime.CompilerServices;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace Carpeddit.App.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly IPropertySet _settings = ApplicationData.Current.LocalSettings.Values;

        public T GetValue<T>(T defaultValue = default, [CallerMemberName] string key = "")
        {
            if (_settings.TryGetValue(key, out object value))
                return (T)value;

            return defaultValue;
        }

        public void SetValue<T>(T value, [CallerMemberName] string key = "")
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
