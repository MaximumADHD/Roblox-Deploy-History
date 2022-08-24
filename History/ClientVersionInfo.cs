using System.Diagnostics.Contracts;

namespace RobloxDeployHistory
{
    public class ClientVersionInfo
    {
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
    }
}
