using System;
using System.Collections.Generic;
using System.Text;

namespace Redis.Stream.Subscriber
{
    public static class StreamParser
    {
        public static IEnumerable<StreamEntry> Parse(StringBuilder stringBuilder)
        {
            int headerSize = 6;
            int dataEntrySize = 8;
            var parsedData = stringBuilder.ToString().Split(CommandConstants.StreamEnd);

            if (parsedData.Length < headerSize || string.IsNullOrEmpty(stringBuilder.ToString()))
            {
                yield break;
            }

            var entriesArray = new ArraySegment<string>(parsedData, headerSize, parsedData.Length - headerSize).ToArray();
            int entryCount = entriesArray.Length / dataEntrySize;

            for (int i = 0; i < entryCount; i++)
            {
                yield return new StreamEntry
                {
                    Id = entriesArray[(i * dataEntrySize) + 1],
                    FieldName = entriesArray[(i * dataEntrySize) + 4],
                    Data = entriesArray[(i * dataEntrySize) + 6]
                };
            }
        }
    }
}