using Xunit;

namespace Redis.Stream.Subscriber.Tests
{
    public class ConnectionTests
    {
        [Fact]
        public void Connect_ReturnsStreamClient_WhenInitialized()
        {
            // Arrange
            var connection = new RedisConnection();

            // Act
            var streamClient = connection.Connect(new RedisStreamSettings
            {
                Host = "localhost",
                Port = 6379
            });

            // Assert
            Assert.NotNull(streamClient);
        }
    }
}