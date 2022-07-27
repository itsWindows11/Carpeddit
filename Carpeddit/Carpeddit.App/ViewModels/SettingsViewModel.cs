using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Carpeddit.App.ViewModels
{
    public class SettingsViewModel : SettingsManager
    {
        public int SetupProgress
        {
            get => Get("Setup", nameof(SetupProgress), 0);
            set => Set("Setup", nameof(SetupProgress), value);
        }

        #region Appearance
        public bool ShowAccountBtnInTitleBar
        {
            get => Get("Appearance", nameof(ShowAccountBtnInTitleBar), true);
            set => Set("Appearance", nameof(ShowAccountBtnInTitleBar), value);
        }

        /// <summary>
        /// A variable that has a value ranging from 0 to 3.
        /// Specifies the color mode that the app uses.
        /// </summary>
        public int ColorMode
        {
            get => Get("Appearance", nameof(ColorMode), 0);
            set => Set("Appearance", nameof(ColorMode), value);
        }

        /// <summary>
        /// The index of the color used for tinting the app background.
        /// </summary>
        public int TintColor
        {
            get => Get("Appearance", nameof(TintColor), 0);
            set => Set("Appearance", nameof(TintColor), value);
        }

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

        public int Theme
        {
            get => Get("Appearance", nameof(Theme), 2);
            set
            {
                Set("Appearance", nameof(Theme), value);
                switch (value)
                {
                    case 0:
                        (Window.Current.Content as Frame).RequestedTheme = ElementTheme.Light;
                        break;
                    case 1:
                        (Window.Current.Content as Frame).RequestedTheme = ElementTheme.Dark;
                        break;
                    case 2:
                        (Window.Current.Content as Frame).RequestedTheme = ElementTheme.Default;
                        break;
                }
            }
        }
        #endregion

        #region Debug
        public bool LoggingEnabled
        {
            get => Get("Debug", nameof(LoggingEnabled), true);
            set => Set("Debug", nameof(LoggingEnabled), value);
        }
        #endregion
    }
}