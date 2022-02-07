using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carpeddit.App.Models
{
    public class CustomAccountModel
    {
        public string Scope { get; set; }
        public string RefreshToken { get; set; }
        public string AccessToken { get; set; }
        public int? TokenExpiresIn { get; set; }
        public bool LoggedIn { get; set; }
        public string DeviceId { get; set; }
    }
}
