namespace Redis.Stream.Subscriber
{
    public static class ClientCommands
    {
        public static string Subscribe(string streamName, int batchSize = 1, long index = 0) {
            // XREAD [COUNT count] [BLOCK milliseconds] STREAMS key [key ...] ID [ID ...]
            return $"XREAD COUNT {batchSize} BLOCK 0 STREAMS {streamName} {index}\r\n";
        }
    }
}