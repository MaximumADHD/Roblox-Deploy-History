namespace RobloxDeployHistory
{
    // This class aggressively enforces channel
    // names being lowercase no matter what.

    public class Channel
    {
        public readonly string Name;

        public Channel(string channel)
        {
            Name = channel.ToLowerInvariant();
        }

        public override string ToString()
        {
            return Name;
        }

        public static implicit operator string(Channel channel)
        {
            return channel.Name;
        }

        public static implicit operator Channel(string name)
        {
            return new Channel(name);
        }
    }
}
