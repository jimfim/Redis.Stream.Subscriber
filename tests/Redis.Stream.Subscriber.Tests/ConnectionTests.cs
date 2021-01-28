using NUnit.Framework;

namespace Redis.Stream.Subscriber.Tests
{
    public class ConnectionTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CanCreateRedisConnection()
        {
            IRedisConnection connection = new RedisConnection();
            var connect = connection.Connect(new RedisStreamSettings
            {
                host = "localhost",
                Port = 6379
            });
            Assert.IsNotNull(connect);
        }
    }
}