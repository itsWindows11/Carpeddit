using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;

namespace Carpeddit.App.Converters
{
    public sealed class BoolToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Determines whether an inverse conversion should take place.
        /// </summary>
        /// <remarks>If set, the value True results in <see cref="Visibility.Collapsed"/>, and false in <see cref="Visibility.Visible"/>.</remarks>
        public bool Reverse { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
            => Convert((bool)value, Reverse);

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => ConvertBack((Visibility)value, Reverse);

        public static Visibility Convert(bool fromValue, bool reverse)
        {
            if (reverse)
                return fromValue ? Visibility.Collapsed : Visibility.Visible;

            return fromValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public static bool ConvertBack(Visibility fromValue, bool reverse)
        {
            if (reverse)
                return fromValue == Visibility.Collapsed;

            return fromValue == Visibility.Visible;
        }
    }
}
