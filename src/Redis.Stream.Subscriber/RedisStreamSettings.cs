using System;

namespace Redis.Stream.Subscriber
{
    public class RedisStreamSettings
    {
        [Obsolete("Use Host instead")]
        public string host
        {
            get => Host;
            set => Host = value;
        }

        public string Host { get; set; }

        public int Port { get; set; } = 6379;

        public int Timeout { get; set; } = 100;
    }
}
