using System;
using System.Collections.Generic;
using System.Text;

namespace Redis.Stream.Subscriber
{
    /// <summary>
    /// Parses Redis stream response data into StreamEntry objects.
    /// </summary>
    public static class StreamParser
    {
        private const int HeaderSize = 6;
        private const int DataEntrySize = 8;

        /// <summary>
        /// Parses a Redis stream response string into individual entries.
        /// Returns empty enumerable if parsing fails or data is invalid.
        /// </summary>
        public static IEnumerable<StreamEntry> Parse(string responseData)
        {
            if (string.IsNullOrWhiteSpace(responseData))
            {
                return Array.Empty<StreamEntry>();
            }

            var parsedData = responseData.Split(CommandConstants.StreamEnd, StringSplitOptions.RemoveEmptyEntries);

            if (parsedData.Length < HeaderSize)
            {
                yield break;
            }

            // Use ArraySegment for better performance without copying array
            var entriesArray = new string[parsedData.Length - HeaderSize];
            Array.Copy(parsedData, HeaderSize, entriesArray, 0, entriesArray.Length);

            if (entriesArray.Length == 0)
            {
                yield break;
            }

            int entryCount = entriesArray.Length / DataEntrySize;

            for (int i = 0; i < entryCount; i++)
            {
                var baseIndex = i * DataEntrySize;
                
                // Validate we have enough elements in this entry
                if (baseIndex + 6 >= entriesArray.Length)
                {
                    yield break;
                }

                yield return new StreamEntry
                {
                    Id = entriesArray[baseIndex + 1] ?? "-",
                    FieldName = entriesArray[baseIndex + 4],
                    Data = entriesArray[baseIndex + 6]
                };
            }
        }

        /// <summary>
        /// Overload that accepts StringBuilder for backward compatibility.
        /// </summary>
        public static IEnumerable<StreamEntry> Parse(StringBuilder? stringBuilder)
        {
            return Parse(stringBuilder?.ToString() ?? string.Empty);
        }
    }
}