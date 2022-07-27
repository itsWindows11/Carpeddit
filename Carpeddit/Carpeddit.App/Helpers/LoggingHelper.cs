using System;

namespace Carpeddit.App.Helpers
{
    public static class LoggingHelper
    {
        public static void LogInfo(string message)
        {
            if (App.SViewModel != null && App.SViewModel.LoggingEnabled)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine(message);
#endif

                App.Logger.Information(message);
            }
        }

        public static void LogError(string message, Exception exception)
        {
            if (App.SViewModel != null && App.SViewModel.LoggingEnabled)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine(message);
                System.Diagnostics.Debug.WriteLine(exception);
#endif

                App.Logger.Error(exception, message);
            }
        }

        public static void LogError(string message)
        {
            if (App.SViewModel != null && App.SViewModel.LoggingEnabled)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine(message);
#endif

                App.Logger.Error(message);
            }
        }
    }
}