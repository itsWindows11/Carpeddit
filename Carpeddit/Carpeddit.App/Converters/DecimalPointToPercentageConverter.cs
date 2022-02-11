using System;
using Windows.UI.Xaml.Data;

namespace Carpeddit.App.Converters
{
    public class DecimalPointToPercentageConverter : IValueConverter
    {
        public bool WithPercentage { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double actualValue = (double)value;
            return $"{Math.Floor(actualValue * 100)}{(WithPercentage ? "%" : "")}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class DecimalPointConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double actualValue = (double)value;
            return $"{Math.Floor(Math.Round(actualValue))}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
