using System.Threading.Tasks;
using Windows.Networking.Connectivity;

namespace Carpeddit.Common.Helpers
{
    public static partial class WebHelpers
    {
        public static Task<bool> CheckIsConnectedAsync()
            => Task.Run(CheckIsConnected);

        private static bool CheckIsConnected()
        {
            var profile = NetworkInformation.GetInternetConnectionProfile();

            if (profile != null)
            {
                return profile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.ConstrainedInternetAccess
                    || profile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
            }

            return false;
        }
    }
}
