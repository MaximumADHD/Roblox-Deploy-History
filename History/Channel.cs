namespace RobloxDeployHistory
{
    // This class aggressively enforces channel
    // names being lowercase no matter what.

    public class Channel
    {
        public readonly string Name;

        public static string Format(string channel)
        {
            return channel.ToLowerInvariant().Trim();
        }

        public Channel(string channel)
        {
            Name = Format(channel);

            switch (Name)
            {
                case "zlive":
                case "roblox":
                {
                    Name = "live";
                    break;
                }

                case "sitetest1.robloxlabs":
                {
                    Name = "zcanary";
                    break;
                }

                case "sitetest2.robloxlabs":
                {
                    Name = "zintegration";
                    break;
                }
            }
        }

        public string BaseUrl
        {
            get
            {
                string baseUrl = "https://setup.rbxcdn.com";

                if (Name != "live")
                    baseUrl += $"channel/{Name}";

                return baseUrl;
            }
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
