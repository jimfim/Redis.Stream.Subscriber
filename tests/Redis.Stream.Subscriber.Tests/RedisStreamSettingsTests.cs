#nullable enable
using System;
using FluentAssertions;
using Xunit;

namespace Redis.Stream.Subscriber.Tests
{
    public class RedisStreamSettingsTests
    {
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
            FluentActions.Invoking(() => settings.Validate()).Should().NotThrow();
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
        public void Validate_ThrowsWhenHostIsInvalid()
        {
            // Arrange
            var settings = new RedisStreamSettings();

            // Act & Assert - invalid host should throw when validated
            FluentActions.Invoking(() => settings.Validate())
                .Should().Throw<ArgumentException>()
                .WithMessage("*Host*");
        }
    }
}
