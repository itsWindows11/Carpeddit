using System.Text.Json.Serialization;

namespace Carpeddit.Models
{
    public sealed class AccountModel
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public string UserName { get; set; }
    }
}
