using System;
using Windows.UI.Xaml.Data;

namespace Carpeddit.App.Converters
{
    public sealed class FormattedNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
            => FormatNumber((int)value);

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        public static string FormatNumber(int num)
        {
            return num switch
            {
                >= 100000000 => (num / 1000000D).ToString("0.#M"),
                >= 1000000 => (num / 1000000D).ToString("0.##M"),
                >= 100000 => (num / 1000D).ToString("0.#K"),
                >= 10000 => (num / 1000D).ToString("0.##k"),
                _ => num.ToString("#,0"),
            };
        }
    }
}
