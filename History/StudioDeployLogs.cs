using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RobloxDeployHistory
{
    public class StudioDeployLogs
    {
        private static readonly NumberFormatInfo NumberFormat = NumberFormatInfo.InvariantInfo;
        private const StringComparison StringFormat = StringComparison.InvariantCulture;

        public bool HasHistory => CurrentLogs.Count > 0;
        public string LastDeployHistory { get; private set; } = "";

        private bool AllowUnsupported = false;
        private static Dictionary<bool, StudioDeployLogs> LogCache = new Dictionary<bool, StudioDeployLogs>();
        
        public HashSet<DeployLog> CurrentLogs { get; private set; } = new HashSet<DeployLog>();

        [Obsolete]
        public HashSet<DeployLog> CurrentLogs_x64 => CurrentLogs;

        [Obsolete]
        public HashSet<DeployLog> CurrentLogs_x86 => CurrentLogs;

        private StudioDeployLogs(bool allowUnsupported)
        {
            AllowUnsupported = allowUnsupported;
            LogCache[allowUnsupported] = this;
        }

        public static async Task<StudioDeployLogs> Get(bool allowUnsupported = false)
        {
            if (!LogCache.TryGetValue(allowUnsupported, out StudioDeployLogs logs))
                logs = new StudioDeployLogs(allowUnsupported);

            string jsonHistory = await HistoryCache.Get();

            if (logs.LastDeployHistory != jsonHistory || logs.AllowUnsupported != allowUnsupported)
            {
                var unsupported = await VersionSupport.GetUnsupportedRangeAsync();
                logs.CurrentLogs.Clear();

                logs.LastDeployHistory = jsonHistory;
                logs.AllowUnsupported = allowUnsupported;

                using (var reader = new StringReader(jsonHistory))
                using (var jsonReader = new JsonTextReader(reader))
                {
                    var data = JObject.Load(jsonReader);

                    foreach (var prop in data.Properties())
                    {
                        var versionId = prop.Name;
                        var versionGuid = prop.Value.ToString();

                        var versionData = versionId.Split('.')
                            .Select(int.Parse)
                            .ToArray();

                        var log = new DeployLog()
                        {
                            VersionGuid = versionGuid,
                            MajorRev = versionData[0],
                            Version = versionData[1],
                            Patch = versionData[2],
                            CommitId = versionData[3],
                        };

                        if (!allowUnsupported)
                            if (log.Version >= unsupported.Min && log.Version <= unsupported.Max)
                                continue;

                        logs.CurrentLogs.Add(log);
                    }
                }
            }

            return logs;
        }

        [Obsolete]
        public static async Task<StudioDeployLogs> Get(Channel channel, bool allowUnsupported = false)
        {
            return await Get(allowUnsupported);
        }

        public static async Task<string> AppendToHistoryLedger(string jsonHistory)
        {
            var liveInfo = await ClientVersionInfo.Get();

            var historyMap = !string.IsNullOrWhiteSpace(jsonHistory)
                ? JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonHistory) ?? new Dictionary<string, string>()
                : new Dictionary<string, string>();

            historyMap[liveInfo.Version] = liveInfo.VersionGuid;

            var orderedPairs = historyMap
                .OrderBy(pair => pair.Key, StringComparer.Ordinal)
                .ToList();

            var output = new JObject();

            foreach (var entry in orderedPairs)
            {
                var prop = new JProperty(entry.Key, entry.Value);
                output.Add(prop);
            }

            using (var writer = new StringWriter())
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                writer.NewLine = "\n";

                jsonWriter.Formatting = Formatting.Indented;
                jsonWriter.IndentChar = ' ';
                jsonWriter.Indentation = 2;

                output.WriteTo(jsonWriter);
                return writer.ToString();
            }
        }
    }
}
