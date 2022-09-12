using System;
using Windows.UI.Xaml.Data;

namespace Carpeddit.App.Converters
{
    public class StringToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
            => App.GetColorFromHex(value as string);

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class StringToTextColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
            => App.GetTextColorFromHex(value as string);

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
