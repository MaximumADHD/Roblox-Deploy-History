using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RobloxDeployHistory
{
    public struct UnsupportedRange
    {
        public uint Min, Max;
    }

    public static class VersionSupport
    {
        private const string Url = "https://raw.githubusercontent.com/MaximumADHD/Roblox-FFlag-Tracker/refs/heads/main/FVariables/FString/S/FStringStudioEmergencyMessageV4.json";
        private static WebClient http = new WebClient();

        public static async Task<UnsupportedRange> GetUnsupportedRangeAsync()
        {
            var range = new UnsupportedRange();

            try
            {
                var json = await http.DownloadStringTaskAsync(Url);

                using (var reader = new StringReader(json))
                using (var jsonReader = new JsonTextReader(reader))
                {
                    var data = JObject.Load(jsonReader);
                    var metadataType = data.Value<string>("Type");
                    string blob = "";

                    switch (metadataType)
                    {
                        case "Unified":
                        {
                            blob = data.Value<string>("Value");
                            break;
                        }
                        case "Channels":
                        {
                            var channel = data.SelectToken("Value");
                            blob = channel.Value<string>("LIVE");
                            break;
                        }
                        case "Platforms":
                        {
                            var platforms = data.SelectToken("Value");
                            blob = platforms.Value<string>("PCStudioApp");
                            break;
                        }
                        case "ChannelsAndPlatforms":
                        {
                            var channels = data.SelectToken("Value");
                            var platform = channels.SelectToken("LIVE");

                            blob = platform.Value<string>("PCStudioApp");
                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(blob))
                    {
                    
                        var buf = Convert.FromBase64String(blob);
                        string supportJson = Encoding.UTF8.GetString(buf);

                        using (var supportReader =  new StringReader(supportJson))
                        using (var jsonSupportReader = new JsonTextReader(supportReader))
                        {
                            var jsonSupportData = JObject.Load(jsonSupportReader);

                            var rawVersionStart = jsonSupportData.Value<string>("versionStart");
                            var rawVersionEnd = jsonSupportData.Value<string>("versionEnd");

                            var versionStart = uint.Parse(rawVersionStart.Split('.')[1]);
                            var versionEnd = uint.Parse(rawVersionEnd.Split('.')[1]);

                            range.Min = versionStart;
                            range.Max = versionEnd;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // TODO: Handle this more explicitly?
                Console.WriteLine($"Could not load version support data: {ex.Message}");
            }

            return range;
        }
    }
}
