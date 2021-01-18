namespace redis_tcp
{
    public class RedisStreamSettings
    {
        public string host { get; set; }
        public int Port { get; set; } = 6379;
        public int Timeout { get; set; } = 100;
        public string Stream { get; set; }
        public int BufferSize { get; set; } = 1024;
        public int BatchSize { get; set; } = 1;
        public int StartingIndex { get; set; } = 0;
    }
}