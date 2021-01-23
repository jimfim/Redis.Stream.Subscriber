namespace Redis.Stream.Subscriber
{
    public class SubscriptionSettings
    {
        public int BufferSize { get; set; } = 1024;
        public int BatchSize { get; set; } = 1;
    }
}