using System;

namespace RobloxDeployHistory
{
    public class DeployLog
    {
        public string VersionGuid { get; set; }

        public int MajorRev { get; set; }
        public int Version { get; set; }
        public int Patch { get; set; }
        public int CommitId { get; set; }

        [Obsolete]
        public int Changelist
        {
            get => CommitId;
            set => CommitId = value;
        }

        public string VersionId => string.Join(".", MajorRev, Version, Patch, CommitId);
        public bool Unsupported { get; set; } = false;

        public override string ToString() => VersionId;
    }
}