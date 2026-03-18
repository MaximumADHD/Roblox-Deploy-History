using System;
using System.Net;
using System.Threading.Tasks;

namespace RobloxDeployHistory
{
    public static class HistoryCache
    {
        private const string HistoryUrl = "https://raw.githubusercontent.com/MaximumADHD/Roblox-Client-Tracker/refs/heads/roblox/version-history.json";

        public static string History { get; private set; }
        public static DateTime LastUpdate { get; private set; }

        public static async Task<string> Get()
        {
            TimeSpan timeDiff = DateTime.Now - LastUpdate;

            if (timeDiff.TotalMinutes > 5)
            {
                using (WebClient http = new WebClient())
                    History = await http.DownloadStringTaskAsync(HistoryUrl);

                LastUpdate = DateTime.Now;
            }

            return History;
        }
    }
}