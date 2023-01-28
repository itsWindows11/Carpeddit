using Carpeddit.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Carpeddit.App.Converters
{
    public sealed class SettingsChoiceToColorConverter : IValueConverter
    {
        private SettingsViewModel SViewModel { get; } = App.Services.GetService<SettingsViewModel>();

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int num = (int)value;
            return new SolidColorBrush(num < 0 ? Colors.Transparent : SViewModel.TintColorsList[num]);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotImplementedException();
    }
}
