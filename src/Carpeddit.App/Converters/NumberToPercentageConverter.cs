using System;
using Windows.UI.Xaml.Data;

namespace Carpeddit.App.Converters
{
    public sealed class NumberToPercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
            => Math.Floor(Math.Round((double)value * 100)) + $"{((string)parameter == "WithPercentageSymbol" ? "%" : string.Empty)}";

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotImplementedException();
    }
}
