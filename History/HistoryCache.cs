using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace RobloxDeployHistory
{
    public class HistoryCache
    {
        private static readonly Dictionary<string, HistoryCache> ChannelCache = new Dictionary<string, HistoryCache>();

        public Channel Channel { get; private set; }
        public string History { get; private set; }
        public DateTime LastUpdate { get; private set; }

        private HistoryCache(Channel channel)
        {
            Channel = channel;
            LastUpdate = DateTime.FromFileTimeUtc(0);
            ChannelCache.Add(channel, this);
        }

        public static async Task<string> GetDeployHistory(Channel channel)
        {
            var deployHistory = Task.Run(() =>
            {
                HistoryCache cache = null;

                if (ChannelCache.ContainsKey(channel))
                    cache = ChannelCache[channel];
                else
                    cache = new HistoryCache(channel);

                lock (cache)
                {
                    TimeSpan timeDiff = DateTime.Now - cache.LastUpdate;

                    if (timeDiff.TotalMinutes > 5)
                    {
                        string historyEndpoint = $"{channel.BaseUrl}/DeployHistory.txt";

                        using (WebClient http = new WebClient())
                            cache.History = http.DownloadString(historyEndpoint);

                        cache.LastUpdate = DateTime.Now;
                    }

                    return cache.History;
                }
            });

            return await deployHistory.ConfigureAwait(false);
        }
    }
}