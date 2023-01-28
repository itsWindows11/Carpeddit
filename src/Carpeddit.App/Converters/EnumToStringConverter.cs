using System;
using Windows.UI.Xaml.Data;

namespace Carpeddit.App.Converters
{
    public sealed class EnumToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
            => value.ToString();

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotImplementedException();
    }
}
