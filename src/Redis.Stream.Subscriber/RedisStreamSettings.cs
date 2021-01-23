namespace Redis.Stream.Subscriber
{
    public class RedisStreamSettings
    {
        public string host { get; set; }
        public int Port { get; set; } = 6379;

        public int Timeout { get; set; } = 100;
    }
}