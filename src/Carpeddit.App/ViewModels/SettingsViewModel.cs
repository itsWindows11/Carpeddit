using Carpeddit.App.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Xaml;

namespace Carpeddit.App.ViewModels
{
    public sealed class SettingsViewModel : ObservableObject
    {
        private ISettingsService _settingsService = App.Services.GetService<ISettingsService>();

        /// <summary>
        /// The setup progress.
        /// </summary>
        public int SetupProgress
        {
            get => _settingsService.GetValue(0);
            set
            {
                OnPropertyChanging();
                _settingsService.SetValue(value);
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Whether the setup has been completed or not.
        /// </summary>
        public bool SetupCompleted
        {
            get => _settingsService.GetValue(false);
            set
            {
                OnPropertyChanging();
                _settingsService.SetValue(value);
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The app theme.
        /// </summary>
        public ElementTheme Theme
        {
            get => (ElementTheme)_settingsService.GetValue((int)ElementTheme.Default);
            set => _settingsService.SetValue((int)value);
        }

        /// <summary>
        /// The type used for tinting.
        /// </summary>
        /// <remarks>
        /// 0 = No tint
        /// 1 = Color
        /// 2 = Image
        /// </remarks>
        public int TintType
        {
            get => _settingsService.GetValue(0);
            set
            {
                OnPropertyChanging();
                _settingsService.SetValue(value);
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The index of the color used for tinting the app background.
        /// </summary>
        public int TintColor
        {
            get => _settingsService.GetValue(-1);
            set
            {
                OnPropertyChanging();
                _settingsService.SetValue(value);
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The URI of the image used for tinting.
        /// </summary>
        public string TintImageUri
        {
            get => _settingsService.GetValue(string.Empty);
            set
            {
                OnPropertyChanging();
                _settingsService.SetValue(value);
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The opacity of the tint used.
        /// </summary>
        public double TintOpacity
        {
            get => _settingsService.GetValue(0.1);
            set
            {
                OnPropertyChanging();
                _settingsService.SetValue(value);
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The list of the available tint colors.
        /// </summary>
        public List<Color> TintColorsList { get; } = new()
        {
            Color.FromArgb(255, 255, 185, 0),
            Color.FromArgb(255, 255, 140, 0),
            Color.FromArgb(255, 247, 99, 12),
            Color.FromArgb(255, 202, 80, 16),
            Color.FromArgb(255, 218, 59, 1),
            Color.FromArgb(255, 239, 105, 80),
            Color.FromArgb(255, 209, 52, 56),
            Color.FromArgb(255, 255, 67, 67),
            Color.FromArgb(255, 231, 72, 86),
            Color.FromArgb(255, 232, 17, 35),
            Color.FromArgb(255, 234, 0, 94),
            Color.FromArgb(255, 195, 0, 82),
            Color.FromArgb(255, 227, 0, 140),
            Color.FromArgb(255, 191, 0, 119),
            Color.FromArgb(255, 194, 57, 179),
            Color.FromArgb(255, 154, 0, 137),
            Color.FromArgb(255, 0, 120, 212),
            Color.FromArgb(255, 0, 99, 177),
            Color.FromArgb(255, 142, 140, 216),
            Color.FromArgb(255, 107, 105, 214),
            Color.FromArgb(255, 135, 100, 184),
            Color.FromArgb(255, 116, 77, 169),
            Color.FromArgb(255, 177, 70, 194),
            Color.FromArgb(255, 136, 23, 152),
            Color.FromArgb(255, 0, 153, 188),
            Color.FromArgb(255, 45, 125, 154),
            Color.FromArgb(255, 0, 183, 195),
            Color.FromArgb(255, 3, 131, 135),
            Color.FromArgb(255, 0, 178, 148),
            Color.FromArgb(255, 1, 133, 116),
            Color.FromArgb(255, 0, 204, 106),
            Color.FromArgb(255, 16, 137, 62),
            Color.FromArgb(255, 122, 117, 116),
            Color.FromArgb(255, 93, 90, 88),
            Color.FromArgb(255, 104, 118, 138),
            Color.FromArgb(255, 81, 92, 107),
            Color.FromArgb(255, 86, 124, 115),
            Color.FromArgb(255, 72, 104, 96),
            Color.FromArgb(255, 73, 130, 5),
            Color.FromArgb(255, 16, 124, 16),
            Color.FromArgb(255, 118, 118, 118),
            Color.FromArgb(255, 76, 74, 72),
            Color.FromArgb(255, 105, 121, 126),
            Color.FromArgb(255, 74, 84, 89),
            Color.FromArgb(255, 100, 124, 100),
            Color.FromArgb(255, 82, 94, 84),
            Color.FromArgb(255, 132, 117, 69),
            Color.FromArgb(255, 126, 115, 95)
        };
    }
}
