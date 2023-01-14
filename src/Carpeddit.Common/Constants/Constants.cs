using Windows.ApplicationModel;
using Carpeddit.Common.Extensions;
using System.Collections.Generic;

namespace Carpeddit.Common.Constants
{
    public static class Constants
    {
        public static string AppVersion { get; } = Package.Current.Id.Version.ToVersionString();

        public static IEnumerable<string> Scopes = new List<string>
        {
            "creddits",
            "modcontributors",
            "modmail",
            "modconfig",
            "subscribe",
            "structuredstyles",
            "vote",
            "wikiedit",
            "mysubreddits",
            "submit",
            "modlog",
            "modposts",
            "modflair",
            "save",
            "modothers",
            "adsconversions",
            "read",
            "privatemessages",
            "report",
            "identity",
            "livemanage",
            "account",
            "modtraffic",
            "wikiread",
            "edit",
            "modwiki",
            "modself",
            "history",
            "flair"
        };
    }
}
