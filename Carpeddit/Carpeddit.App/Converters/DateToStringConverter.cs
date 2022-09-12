using Carpeddit.App.Helpers;
using System;
using Windows.UI.Xaml.Data;

namespace Carpeddit.App.Converters
{
    public class DateToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
            => PostHelpers.GetRelativeDate((DateTime)value);

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
