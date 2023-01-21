using System;
using System.Net;
using Windows.UI.Xaml.Data;

namespace Carpeddit.App.Converters
{
    public class StringToUriConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool successful = Uri.TryCreate(WebUtility.HtmlDecode(value as string), UriKind.Absolute, out Uri result);

            if (successful)
                return result;

            return new Uri("ms-appx:///Assets/DefaultSubredditIcon.png");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (value as Uri).AbsoluteUri;
        }
    }
}
