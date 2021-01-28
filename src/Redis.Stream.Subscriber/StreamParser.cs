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
            int dataSize = 8;
            var parsedStreamData = stringBuilder.ToString().Split(CommandConstants.StreamEnd);
            var x = parsedStreamData.Length - headerSize;
            if (x < 0)
            {
                yield return new StreamEntry();
            }
            else
            {
                var myArrSegMid = new ArraySegment<string>( parsedStreamData,headerSize,parsedStreamData.Length - headerSize).ToArray();
                var dataSegments = myArrSegMid.Length / 8;
                for (int i = 0; i < dataSegments; i++)
                {
                    var entry = new StreamEntry
                    {
                        Id = myArrSegMid[(i*dataSize)+1],
                        FieldName = myArrSegMid[(i*dataSize)+4],
                        Data = myArrSegMid[(i*dataSize)+6]
                    };
                    yield return entry;
                }
            }
        }
    }
}