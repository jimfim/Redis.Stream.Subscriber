using System;
using Xunit;

namespace Redis.Stream.Subscriber.Tests
{
    public class SubscriptionSettingsTests
    {
        [Fact]
        public void Constructor_SetsValidDefaults()
        {
            // Arrange & Act
            var settings = new SubscriptionSettings();

            // Assert
            Assert.Equal(1024, settings.BufferSize);
            Assert.Equal(1, settings.BatchSize);
        }

        [Fact]
        public void Validate_WithValidSettings_DoesNotThrow()
        {
            // Arrange
            var settings = new SubscriptionSettings();

            // Act & Assert - should not throw
            Assert.DoesNotThrow(() => settings.Validate());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void BufferSize_Setter_WithInvalidValue_ThrowsArgumentException(int value)
        {
            // Arrange
            var settings = new SubscriptionSettings();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => settings.BufferSize = value);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void BatchSize_Setter_WithInvalidValue_ThrowsArgumentException(int value)
        {
            // Arrange
            var settings = new SubscriptionSettings();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => settings.BatchSize = value);
        }

        [Theory]
        [InlineData(1, 64)]
        [InlineData(64, 1024)]
        [InlineData(1000, 8192)]
        public void SetValidValues_BufferAndBatchSizesAreSetCorrectly(int bufferSize, int batchSize)
        {
            // Arrange
            var settings = new SubscriptionSettings();

            // Act
            settings.BufferSize = bufferSize;
            settings.BatchSize = batchSize;

            // Assert
            Assert.Equal(bufferSize, settings.BufferSize);
            Assert.Equal(batchSize, settings.BatchSize);
        }

        [Fact]
        public void Validate_WithInvalidBufferSize_ThrowsArgumentException()
        {
            // Arrange
            var settings = new SubscriptionSettings();
            settings.BufferSize = -1;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => settings.Validate());
        }

        [Fact]
        public void Validate_WithInvalidBatchSize_ThrowsArgumentException()
        {
            // Arrange
            var settings = new SubscriptionSettings();
            settings.BatchSize = 0;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => settings.Validate());
        }
    }
}
