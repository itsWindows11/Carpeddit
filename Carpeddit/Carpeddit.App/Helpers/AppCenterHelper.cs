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
            try
            {
                var file = await StorageFile.GetFileFromApplicationUriAsync(new("ms-appx:///AppCenterKey.txt"));
                var text = await FileIO.ReadTextAsync(file);

                if (text.Length > 0)
                {
                    AppCenter.Start(text, typeof(Analytics), typeof(Crashes));
                    LoggingHelper.LogInfo("[AppCenterHelper] App Center initialized successfully.");
                }
                else
                    LoggingHelper.LogInfo("[AppCenterHelper] App Center secret wasn't provided when building the app, it won't be initialized.");
            }
            catch (Exception e)
            {
                LoggingHelper.LogError("[AppCenterHelper] App Center failed to initialize.", e);
            }
        }
    }
}
