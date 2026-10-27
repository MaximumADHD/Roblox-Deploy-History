using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;

using Newtonsoft.Json;

namespace RobloxDeployHistory
{
    // This fetches build information off of the build-archive GitHub project
    // being hosted by Anaminus, which has a more complete paper trail of history.

    public enum BuildGroup
    {
        Legacy,
        Production,
    }

    public class BuildRecord
    {
        public string Guid;
        public DateTime Date;
        public string Version;
    }

    public class BuildMetadata
    {
        public List<string> Files;
        public List<BuildRecord> Builds;
        public Dictionary<string, List<string>> Missing;
    }

    public static class BuildArchive
    {
        private const string BASE_URL = "https://raw.githubusercontent.com/RobloxAPI/build-archive/refs/heads/master";

        public static async Task<BuildMetadata> GetBuildMetadata(BuildGroup group = BuildGroup.Legacy)
        {
            using var http = new HttpClient();

            string groupName = Enum.GetName(group).ToLowerInvariant();
            string url = $"{BASE_URL}/data/{groupName}/metadata.json";

            var json = await http.GetStringAsync(url);
            return JsonConvert.DeserializeObject<BuildMetadata>(json);
        }

        public static async Task<string> GetFile(string guid, string file, BuildGroup group = BuildGroup.Legacy)
        {
            using var http = new HttpClient();

            string groupName = Enum.GetName(group).ToLowerInvariant();
            string url = $"{BASE_URL}/data/{groupName}/builds/{guid}/{file}";

            return await http.GetStringAsync(url);
        }
    }
}
