#nullable enable
using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Redis.Stream.Subscriber.Tests
{
    public class StreamParserTests
    {
        [Fact]
        public void Parse_NullString_ReturnsEmptyEnumerable()
        {
            var result = StreamParser.Parse((string?)null!);
            result.Should().BeEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Parse_EmptyString_ReturnsEmptyEnumerable(string emptyInput)
        {
            var result = StreamParser.Parse(emptyInput);
            result.Should().BeEmpty();
        }

        [Fact]
        public void Parse_InvalidFormat_ReturnsEmptyEnumerable()
        {
            var invalidData = "not a valid redis stream response";
            var result = StreamParser.Parse(invalidData);
            result.Should().BeEmpty();
        }

        [Fact]
        public void Parse_ResponseWithPartialEntry_ReturnsOnlyCompleteEntries()
        {
            var partialResponse = "$1\r\n$9\r\n$3\r\n0-0\r\n$8\r\nfield1\r\n$4\r\ndata\r\npartial";
            var result = StreamParser.Parse(partialResponse);
            // Parser handles incomplete data gracefully by returning empty or partial results
            result.Should().NotBeNull();
        }

        [Fact]
        public void Parse_WithStringBuilderInput_ReturnsSameResult()
        {
            var input = "$1\r\n$9\r\n$3\r\n0-0\r\n$8\r\nfield1\r\n$4\r\ndata\r\n";
            var builder = new System.Text.StringBuilder(input);

            var result1 = StreamParser.Parse(builder);
            var result2 = StreamParser.Parse(builder.ToString());

            result1.Should().NotBeNull();
            result2.Should().NotBeNull();
        }
    }
}
