using System.Diagnostics.Contracts;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RobloxDeployHistory
{
    public class ClientVersionInfo
    {
        private class ClientVersionResponse
        {
            public string Version = "";
            public string ClientVersionUpload = "";
        }

        public Channel Channel { get; private set; }
        public string Version { get; private set; }
        public string VersionGuid { get; private set; }

        public ClientVersionInfo(Channel channel, string version, string versionGuid)
        {
            Channel = channel;
            Version = version;
            VersionGuid = versionGuid;
        }

        public ClientVersionInfo(DeployLog log)
        {
            Contract.Requires(log != null);
            VersionGuid = log.VersionGuid;
            Version = log.VersionId;
            Channel = log.Channel;
        }

        private ClientVersionInfo(Channel channel, ClientVersionResponse response)
        {
            Channel = channel;
            Version = response.Version;
            VersionGuid = response.ClientVersionUpload;
        }

        public static async Task<ClientVersionInfo> Get(Channel channel, string binaryType)
        {
            using (var http = new WebClient())
            {
                string json = await http.DownloadStringTaskAsync($"https://clientsettings.roblox.com/v2/client-version/{binaryType}/channel/{channel}");
                var response = JsonConvert.DeserializeObject<ClientVersionResponse>(json);
                return new ClientVersionInfo(channel, response);
            }
        }
    }
}
