namespace Redis.Stream.Subscriber
{
    public static class CommandConstants
    {
        public static string StreamEnd => "\r\n";

        public static string Subscribe(string streamName, int batchSize = 1, long index = 0)
        {
            // XREAD [COUNT count] [BLOCK milliseconds] STREAMS key [key ...] ID [ID ...]
            return $"XREAD COUNT {batchSize} BLOCK 0 STREAMS {streamName} {index}{StreamEnd}";
        }

        public static string ReadRangeBackwards(string streamName, int batchSize = 1, string id = "-")
        {
            // XREVRANGE key start stop [COUNT count]
            return $"XREVRANGE {streamName} {id} - COUNT {batchSize}{StreamEnd}";
        }
    }
}
