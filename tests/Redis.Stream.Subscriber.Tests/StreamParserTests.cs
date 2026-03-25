using System;
using Xunit;

namespace Redis.Stream.Subscriber.Tests
{
    public class StreamParserTests
    {
        [Fact]
        public void Parse_NullString_ReturnsEmptyEnumerable()
        {
            // Act
            var result = StreamParser.Parse(null!);

            // Assert
            Assert.Empty(result);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Parse_EmptyString_ReturnsEmptyEnumerable(string emptyInput)
        {
            // Act
            var result = StreamParser.Parse(emptyInput);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void Parse_InvalidFormat_ReturnsEmptyEnumerable()
        {
            // Arrange
            var invalidData = "not a valid redis stream response";

            // Act
            var result = StreamParser.Parse(invalidData);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void Parse_ValidResponse_ReturnsCorrectEntries()
        {
            // Arrange - Simulating Redis XREAD response format:
            // $1\r\n (array length)
            // $9\r\n (entry count)
            // $3\r\n (ID length)
            // 0-0\r\n (actual ID)
            // $8\r\n (field name length)
            // field1\r\n (actual field name)
            // $4\r\n (data length)
            // data\r\n (actual data)

            var validResponse = "$1\r\n$9\r\n$3\r\n0-0\r\n$8\r\nfield1\r\n$4\r\ndata\r\n";

            // Act
            var result = StreamParser.Parse(validResponse).ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal("0-0", result[0].Id);
            Assert.Equal("field1", result[0].FieldName);
            Assert.Equal("data", result[0].Data);
        }

        [Fact]
        public void Parse_MultipleEntries_ReturnsAllEntries()
        {
            // Arrange - Two entries in one response
            var multiEntryResponse = "$1\r\n$9\r\n$3\r\n0-0\r\n$8\r\nfield1\r\n$4\r\ndata1\r\n$3\r\n0-1\r\n$8\r\nfield2\r\n$4\r\ndata2\r\n";

            // Act
            var result = StreamParser.Parse(multiEntryResponse).ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("0-0", result[0].Id);
            Assert.Equal("field1", result[0].FieldName);
            Assert.Equal("data1", result[0].Data);
            Assert.Equal("0-1", result[1].Id);
            Assert.Equal("field2", result[1].FieldName);
            Assert.Equal("data2", result[1].Data);
        }

        [Fact]
        public void Parse_WithStringBuilderInput_ReturnsSameResult()
        {
            // Arrange
            var input = "$1\r\n$9\r\n$3\r\n0-0\r\n$8\r\nfield1\r\n$4\r\ndata\r\n";
            var builder = new System.Text.StringBuilder(input);

            // Act
            var result1 = StreamParser.Parse(builder).ToList();
            var result2 = StreamParser.Parse(builder.ToString()).ToList();

            // Assert
            Assert.Equal(result1.Count, result2.Count);
        }

        [Fact]
        public void Parse_ResponseWithPartialEntry_ReturnsOnlyCompleteEntries()
        {
            // Arrange - Response with incomplete final entry (missing fields)
            var partialResponse = "$1\r\n$9\r\n$3\r\n0-0\r\n$8\r\nfield1\r\n$4\r\ndata\r\npartial";

            // Act
            var result = StreamParser.Parse(partialResponse).ToList();

            // Assert - Only the complete first entry should be returned
            Assert.Single(result);
        }
    }
}
