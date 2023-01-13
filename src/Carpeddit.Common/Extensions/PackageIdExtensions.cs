using Windows.ApplicationModel;

namespace Carpeddit.Common.Extensions
{
    public static class PackageIdExtensions
    {
        /// <summary>
        /// Converts the provided <see cref="PackageVersion" /> to a <see cref="string" />
        /// </summary>
        /// <param name="version">The package version to convert.</param>
        /// <returns>The string that represents the package version, for example: 1.2.3.0</returns>
        public static string ToVersionString(this PackageVersion version)
            => $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
    }
}
