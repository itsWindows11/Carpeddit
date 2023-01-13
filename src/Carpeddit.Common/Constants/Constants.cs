using Windows.ApplicationModel;
using Carpeddit.Common.Extensions;

namespace Carpeddit.Common.Constants
{
    public static class Constants
    {
        public static string AppVersion { get; } = Package.Current.Id.Version.ToVersionString();
    }
}
