using System;
using FluentAssertions;
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
            settings.BufferSize.Should().Be(1024);
            settings.BatchSize.Should().Be(1);
        }

        [Fact]
        public void Validate_WithValidSettings_DoesNotThrow()
        {
            // Arrange
            var settings = new SubscriptionSettings();

            // Act & Assert - should not throw
            FluentActions.Invoking(() => settings.Validate()).Should().NotThrow();
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

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void BufferSize_Setter_ThrowsOnInvalidValue(int value)
        {
            var settings = new SubscriptionSettings();
            
            FluentActions.Invoking(() => settings.BufferSize = value)
                .Should().Throw<ArgumentException>()
                .WithMessage("*BufferSize*");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void BatchSize_Setter_ThrowsOnInvalidValue(int value)
        {
            var settings = new SubscriptionSettings();
            
            FluentActions.Invoking(() => settings.BatchSize = value)
                .Should().Throw<ArgumentException>()
                .WithMessage("*BatchSize*");
        }

        [Fact]
        public void Validate_ThrowsOnInvalidBufferSize()
        {
            // Arrange - use reflection to set invalid value bypassing setter validation
            var settings = new SubscriptionSettings();
            var fieldInfo = typeof(SubscriptionSettings).GetField("_bufferSize", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            fieldInfo?.SetValue(settings, -1);

            // Act & Assert
            FluentActions.Invoking(() => settings.Validate())
                .Should().Throw<ArgumentException>()
                .WithMessage("*BufferSize*");
        }

        [Fact]
        public void Validate_ThrowsOnInvalidBatchSize()
        {
            // Arrange - use reflection to set invalid value bypassing setter validation
            var settings = new SubscriptionSettings();
            var fieldInfo = typeof(SubscriptionSettings).GetField("_batchSize", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            fieldInfo?.SetValue(settings, -1);

            // Act & Assert
            FluentActions.Invoking(() => settings.Validate())
                .Should().Throw<ArgumentException>()
                .WithMessage("*BatchSize*");
        }
    }
}
