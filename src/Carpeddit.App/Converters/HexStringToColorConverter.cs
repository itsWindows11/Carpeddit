using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Carpeddit.App.Converters
{
    public sealed class HexStringToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string hex)
            {
                hex = hex.Replace("#", string.Empty);

                try
                {
                    byte r = (byte)System.Convert.ToUInt32(hex.Substring(0, 2), 16);
                    byte g = (byte)System.Convert.ToUInt32(hex.Substring(2, 2), 16);
                    byte b = (byte)System.Convert.ToUInt32(hex.Substring(4, 2), 16);

                    return Color.FromArgb(255, r, g, b);
                }
                catch
                {

                }
            }


            return Application.Current.RequestedTheme == ApplicationTheme.Light ? Color.FromArgb(255, 255, 255, 255) : Color.FromArgb(255, 0, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
