using Carpeddit.App.Services;
using Microsoft.Extensions.DependencyInjection;
using Windows.UI.Xaml;

namespace Carpeddit.App.ViewModels
{
    public sealed class SettingsViewModel
    {
        private ISettingsService _settingsService = App.Services.GetService<ISettingsService>();

        public int SetupProgress
        {
            get => _settingsService.GetValue(0);
            set => _settingsService.SetValue(value);
        }

        public bool SetupCompleted
        {
            get => _settingsService.GetValue(false);
            set => _settingsService.SetValue(value);
        }

        public ElementTheme Theme
        {
            get => (ElementTheme)_settingsService.GetValue((int)ElementTheme.Default);
            set => _settingsService.SetValue((int)value);
        }
    }
}
