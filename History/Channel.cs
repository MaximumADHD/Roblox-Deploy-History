namespace RobloxDeployHistory
{
    // This class aggressively enforces channel names 
    // being lowercase no matter what. It also handles
    // implicit loose string equality of channel names,
    // and provides backwards compatibility for branches.

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
                case "sitetest3.robloxlabs":
                {
                    Name = "zintegration";
                    break;
                }
            }
        }

        public override bool Equals(object obj)
        {
            Channel channel = obj.ToString();
            return Name == channel.Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public string BaseUrl
        {
            get
            {
                string baseUrl = "https://setup.rbxcdn.com";

                if (Name != "live")
                    baseUrl += $"/channel/{Name}";

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
