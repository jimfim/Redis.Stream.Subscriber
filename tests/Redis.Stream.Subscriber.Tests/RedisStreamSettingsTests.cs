using System;
using Xunit;

namespace Redis.Stream.Subscriber.Tests
{
    public class RedisStreamSettingsTests
    {
        [Fact]
        public void Constructor_SetsDefaults_PropertiesInitialized()
        {
            // Arrange & Act
            var settings = new RedisStreamSettings();

            // Assert
            Assert.Equal(6379, settings.Port);
            Assert.Equal(100, settings.Timeout);
        }

        [Fact]
        public void Validate_WithValidSettings_DoesNotThrow()
        {
            // Arrange
            var settings = new RedisStreamSettings
            {
                Host = "localhost",
                Port = 6379,
                Timeout = 100
            };

            // Act & Assert - should not throw
            Assert.DoesNotThrow(() => settings.Validate());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Validate_WithInvalidHost_ThrowsArgumentException(string? host)
        {
            // Arrange
            var settings = new RedisStreamSettings
            {
                Host = host,
                Port = 6379,
                Timeout = 100
            };

            // Act & Assert
            Assert.ThrowsAny<ArgumentException>(() => settings.Validate());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(65536)]
        public void Validate_WithInvalidPort_ThrowsArgumentException(int port)
        {
            // Arrange
            var settings = new RedisStreamSettings
            {
                Host = "localhost",
                Port = port,
                Timeout = 100
            };

            // Act & Assert
            Assert.ThrowsAny<ArgumentException>(() => settings.Validate());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-100)]
        public void Validate_WithInvalidTimeout_ThrowsArgumentException(int timeout)
        {
            // Arrange
            var settings = new RedisStreamSettings
            {
                Host = "localhost",
                Port = 6379,
                Timeout = timeout
            };

            // Act & Assert
            Assert.ThrowsAny<ArgumentException>(() => settings.Validate());
        }

        [Fact]
        public void host_Property_ThrowsInvalidOperationException()
        {
            // Arrange
            var settings = new RedisStreamSettings();

            // Act & Assert - obsolete property should throw when accessed
            Assert.Throws<InvalidOperationException>(() => _ = settings.host);
        }
    }
}
