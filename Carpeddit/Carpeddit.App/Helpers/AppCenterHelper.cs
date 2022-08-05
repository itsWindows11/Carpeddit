using System;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System.Threading.Tasks;
using Windows.Storage;

namespace Carpeddit.App.Helpers
{
    public static class AppCenterHelper
    {
        public static async Task StartAppCenterAsync()
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new("ms-appx:///AppCenterKey.txt"));
            AppCenter.Start(await FileIO.ReadTextAsync(file), typeof(Analytics), typeof(Crashes));
        }
    }
}
