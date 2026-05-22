using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RobloxDeployHistory
{
    public class ClientVersionHttpError
    {
        public int Code = 0;
        public string Message = "";
    }

    public class ClientVersionInfo
    {
        private class ClientVersionResponse
        {
            public string Version = "";
            public string ClientVersionUpload = "";

            public List<ClientVersionHttpError> Errors = new List<ClientVersionHttpError>();
            public bool Success => !Errors.Any();
        }

        public bool Success => !Errors.Any();
        public List<ClientVersionHttpError> Errors { get; private set; }

        public string Version { get; private set; }
        public string VersionGuid { get; private set; }
        public string ChannelName { get; private set; }


        public ClientVersionInfo(string version, string versionGuid)
        {
            Version = version;
            VersionGuid = versionGuid;
            Errors = new List<ClientVersionHttpError>();
        }

        public ClientVersionInfo(DeployLog log)
        {
            Version = log.VersionId;
            VersionGuid = log.VersionGuid;
            Errors = new List<ClientVersionHttpError>();
        }

        private ClientVersionInfo(ClientVersionResponse response, string channel = "LIVE")
        {
            Errors = response.Errors;
            Version = response.Version;
            VersionGuid = response.ClientVersionUpload;
            ChannelName = channel;
        }

        public static async Task<ClientVersionInfo> Get(string channel = "LIVE", string binaryType = "WindowsStudio64", string channelToken = "")
        {
            using (var http = new WebClient())
            {
                if (!string.IsNullOrEmpty(channelToken)) http.Headers.Add("roblox-channel-token", channelToken);
                string url = $"https://clientsettings.roblox.com/v2/client-version/{binaryType}";

                if (channel.ToLowerInvariant() != "live")
                    url += $"/channel/{channel}";

                string json = await http.DownloadStringTaskAsync(url);
                var response = JsonConvert.DeserializeObject<ClientVersionResponse>(json);
                return new ClientVersionInfo(response, channel);
            }
        }

        [Obsolete]
        public static async Task<ClientVersionInfo> Get(Channel channel, string binaryType = "WindowsStudio64")
        {
            return await Get(binaryType, channel);
        }
    }
}
