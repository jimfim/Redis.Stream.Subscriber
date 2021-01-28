namespace Redis.Stream.Subscriber
{
    public interface IRedisConnection
    {
        IRedisStreamClient Connect(RedisStreamSettings settings);

        void Close();
    }
}